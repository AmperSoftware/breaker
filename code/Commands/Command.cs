using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class Command
	{
		public struct CommandInfo
		{
			public CommandAttribute Attribute;
			public MethodDescription Method;

			public CommandInfo( CommandAttribute attribute, MethodDescription method )
			{
				Attribute = attribute;
				Method = method;
			}
		}
		private static Dictionary<string, CommandInfo> commands = new();
		public static IReadOnlyDictionary<string, CommandInfo> All => commands.AsReadOnly();
		/// <summary>
		/// Deletes the current list of commands and generates a new one.
		/// </summary>
		[Event.Hotload]
		[Event.Entity.PostSpawn]
		public static void LoadAll()
		{
			commands.Clear();

			var types = TypeLibrary.GetTypes();
			foreach ( var type in types )
			{
				foreach ( var method in type.Methods )
				{
					if ( !method.IsStatic || method.IsProperty )
						continue;

					var cmdAttributes = method.Attributes.OfType<CommandAttribute>();
					if ( !cmdAttributes.Any() )
						continue;
					
					var attribute = cmdAttributes.First();
					var name = attribute.Name;
					if ( commands.ContainsKey( name ) )
					{
						Log.Error( $"Tried to register command with duplicate name {name}!" );
						continue;
					}

					Debug.Log( $"Registering command {name}." );
					commands.Add( name, new( attribute, method ) );
				}
			}
		}

		/// <summary>
		/// Executes a command.
		/// </summary>
		/// <param name="client">The client that executed the command.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="args">The arguments to pass to the command.</param>
		public static void Execute( string command, IClient client, string[] args)
		{
			if ( !commands.TryGetValue( command, out var info ) )
			{
				Log.Error( $"Tried to execute command {command} but it doesn't exist!" );
				return;
			}
			
			var method = info.Method;
			var permissions = info.Method.Attributes.OfType<PermissionAttribute>();
			if(permissions.Any())
			{
				foreach(var perm in permissions)
				{
					if ( !client.HasPermission( perm ) )
					{
						Log.Error( $"Tried to execute command {command} but the client doesn't have the permission {perm.Permission}!" );
						return;
					}
				}
			}
			
			ParameterInfo[] parameters = method.Parameters;
			
			int parameterCount = parameters.Length;
			int requiredParameters = parameterCount - parameters.Count( p => p.DefaultValue != null );
			if(requiredParameters > 0 || parameterCount > 0 && args.Length > 0)
			{
				if(args == default)
				{
					Log.Error( $"Tried to execute command {command} but it requires arguments!" );
					return;
				}

				int argsCount = args.Length;
				if ( parameterCount != argsCount )
				{
					Log.Error( $"Tried to execute command {command} but the parameter count doesn't match the argument count!" );
					return;
				}

				var parameterTypes = parameters.Select( p => p.ParameterType );
				var parameterValues = new object[parameterCount];
				for ( int i = 0; i < parameterCount; i++ )
				{
					var type = parameterTypes.ElementAt( i );
					var argument = args[i];

					object value = null;
					if ( !type.IsAssignableFrom( argument.GetType() ) )
					{
						value = Convert.ChangeType( argument, type );
						if ( !type.IsAssignableFrom( value.GetType() ) )
						{
							Log.Error( $"Tried to execute command {command} but the argument {argument} couldn't be converted to {type}!" );
							return;
						}
					}
					else
					{
						value = argument;
					}

					parameterValues[i] = value;
				}

				Debug.Log( $"Executing {command} with parameters" );
				foreach(var p in parameters)
				{
					Debug.Log( $"- {p}" );
				}

				method.Invoke( null, parameterValues );
			}
			else
			{
				method.Invoke( null, null );
			}
		}
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

			return cmd.Method.Attributes.OfType<PermissionAttribute>().ToArray();
		}
	}
	[AttributeUsage( AttributeTargets.Method )]
	public class CommandAttribute : Attribute
	{
		public string Name { get; set; }

		public CommandAttribute(string name)
		{
			Name = name;
		}
	}
}
