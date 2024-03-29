﻿using Breaker;
using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Breaker;

public static partial class Command
{
	public struct ContextInfo
	{
		public IClient Caller;
		public float CallTick;
		public DateTime CallTime;

		public ContextInfo( IClient caller )
		{
			Caller = caller;
			CallTick = Time.Tick;
			CallTime = DateTime.Now;
		}
	}
	public class Info
	{
		public string Name = "";
		public string Group = "";
		public CommandAttribute Attribute;
		public MethodDescription Method;

		public Info( CommandAttribute attribute, MethodDescription method )
		{
			Attribute = attribute;
			Method = method;
		}

		public ParameterInfo[] GetParameters() => Method.Parameters;
	}
	public class ClientInfo
	{
		public string Key;
		public string Name;
		public string Group;
		public string[] Parameters;

		public ClientInfo( string key, string name, string group = "", params string[] parameters )
		{
			Key = key;
			Name = name;
			Group = group;
			Parameters = parameters;
		}

		public static implicit operator ClientInfo( Info info )
		{
			return new ClientInfo( info.Attribute.Key, info.Name, info.Group, info.GetParameters().Select(p => p.Name).ToArray() );
		}
	}
	private static Dictionary<string, Info> commands = new();
	private static Dictionary<string, Info> aliasToCommand = new();

	private static List<ClientInfo> clientCommands = new();
	/// <summary>
	/// All current commands. Only usable on the server.
	/// </summary>
	public static IReadOnlyDictionary<string, Info> All => commands.AsReadOnly();
	public static ContextInfo Context { get; private set; }
	public static IClient Caller => Context.Caller;
	public static string CallerName => Caller?.Name ?? "Console";
	/// <summary>
	/// Deletes the current list of commands and generates a new one.
	/// </summary>
	[BRKEvent.ConfigLoaded]
	public static void LoadAll()
	{
		commands.Clear();
		aliasToCommand.Clear();
		NetworkCommandClear();

		var types = TypeLibrary.GetTypes();
		foreach ( var type in types )
		{
			foreach ( var method in type.Methods )
			{
				if ( method.IsStatic && method.IsMethod )
					LoadMethod( method, type );
			}
		}
	}

	private static void LoadMethod(MethodDescription method, TypeDescription type)
	{
		var attribute = method.GetCustomAttribute<CommandAttribute>();

		if ( Console.Vars.breaker_autogen_builtin_commands && attribute == null )
		{
			var builtinAttrib = method.GetCustomAttribute<ConCmd.AdminAttribute>();
			if(builtinAttrib != null)
			{
				string attributeName = builtinAttrib.Name ?? method.Name.ToLowerInvariant();
				string source = type.Namespace?.ToLowerInvariant() ?? type.Name.ToLowerInvariant();
				attribute = new CommandAttribute( attributeName ) { Description = builtinAttrib.Help, GeneratedFrom = source };
				Log.Info( attribute.IsGenerated );
			}
		}

		if ( attribute == null )
			return;

		var name = attribute.Key;

		// Get group from method or type
		string group = null;

		if ( !string.IsNullOrWhiteSpace( method.Group ) )
			group = method.Group;
		else if ( !string.IsNullOrWhiteSpace( type.Group ) )
			group = type.Group;

		string title = name;
		if ( !string.IsNullOrWhiteSpace(method.Title) )
		{
			title = method.Title;
		}

		Info info = new( attribute, method ) { Name = title, Group = group };
		if ( commands.ContainsKey( name ) )
		{
			Logging.Error( $"Tried to register command with duplicate name {name}!" );
			return;
		}

		foreach ( var alias in attribute.Aliases )
		{
			if ( commands.ContainsKey( alias ) )
			{
				Logging.Error( $"Tried to register command with alias {alias} which already exists as a command!" );
			}
			else if ( aliasToCommand.ContainsKey( alias ) )
			{
				Logging.Error( $"Tried to register command alias {alias} which already exists!" );
				continue;
			}

			Debug.Log( $"Alias for {name}: {alias}" );
			aliasToCommand.Add( alias, info );
		}

		Debug.Log( $"Registering command {name}." );
		commands.Add( name, info );
		NetworkCommandInfo( name, title, group, method.Parameters.Select( p => p.Name ).ToArray() );
	}

	#region Networking
	[GameEvent.Server.ClientJoined]
	static void OnClientJoin( ClientJoinedEvent ev )
	{
		var client = ev.Client;
		foreach(var kv in commands)
		{
			var cmd = kv.Value;
			NetworkCommandInfo( To.Single( client ), kv.Key, cmd.Name, cmd.Group, cmd.GetParameters().Select(p => p.Name).ToArray() );
		}
		
	}
	[ClientRpc]
	public static void NetworkCommandInfo(string key, string name, string group = "", string[] parameters = default )
	{
		ClientInfo info = new( key, name, group, parameters );
		clientCommands.Add( info );
	}
	[ClientRpc]
	public static void NetworkCommandClear()
	{
		clientCommands.Clear();
	}
	#endregion

	public static ParameterInfo[] Parameters( string name )
	{
		if(!commands.TryGetValue(name, out var cmd))
		{
			Log.Error( $"Tried to get parameters of command {name} which does not exist!" );
			return default;
		}

		return cmd.Method.Parameters;
	}
	public static PermissionAttribute[] Permissions(string name)
	{
		if ( !commands.TryGetValue( name, out var cmd ) )
		{
			Log.Error( $"Tried to get required permissions of command {name} which does not exist!" );
			return default;
		}

		if ( cmd.Attribute.IsGenerated )
		{
			return new PermissionAttribute[] { new($"other.{cmd.Attribute.GeneratedFrom}.{name}") };
		}

		return cmd.Method.Attributes.OfType<PermissionAttribute>().ToArray();
	}

	public static IEnumerable<ClientInfo> GetAllClient()
	{
		return clientCommands;
	}

	public static IEnumerable<IGrouping<string, ClientInfo>> GetAllClientGrouped()
	{
		return clientCommands.GroupBy( info => info.Group );
	}
}
[AttributeUsage( AttributeTargets.Method )]
public class CommandAttribute : Attribute
{
	public string Key { get; set; }
	public string[] Aliases { get; }
	public string Description { get; set; }
	internal string GeneratedFrom = "";
	internal bool IsGenerated => !string.IsNullOrEmpty( GeneratedFrom );
	public CommandAttribute(string key, params string[] aliases)
	{
		if ( string.IsNullOrEmpty( key ) )
			throw new ArgumentNullException( nameof(key) );

		Key = key;
		Aliases = aliases;
	}
}
