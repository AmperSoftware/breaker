using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Breaker
{
	public static class Command
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
		public static ContextInfo Context { get; private set; }
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
		/// <param name="caller">The client that executed the command.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="args">The arguments to pass to the command.</param>
		public static void Execute( string command, IClient caller, string[] args)
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
					if ( !caller.HasPermission( perm ) )
					{
						Log.Error( $"Tried to execute command {command} but the client doesn't have the permission {perm.Permission}!" );
						return;
					}
				}
			}
			ParameterInfo[] parameters = method.Parameters;
			
			int parameterCount = parameters.Length;
			int requiredParameters = parameterCount - parameters.Count( p => p.IsOptional || p.ParameterType == typeof(ContextInfo));
			if(parameterCount > 0)
			{
				if(args == default)
				{
					Log.Error( $"Tried to execute command {command} but it requires arguments!" );
					return;
				}

				int argsCount = args.Length;
				if ( parameterCount < argsCount || argsCount < requiredParameters )
				{
					Log.Error( $"Tried to execute command {command} but the parameter count doesn't match the argument count!" );
					return;
				}
				var parameterTypes = parameters.Select( p => p.ParameterType );
				var parameterValues = new object[parameterCount];
				for ( int i = 0; i < parameterCount; i++ )
				{
					var type = parameterTypes.ElementAt( i );

					Debug.Log( $"Element {i} in {argsCount}" );
					if ( i >= argsCount )
					{
						Debug.Log( $"Using default value" );
						parameterValues[i] = parameters.ElementAt( i ).DefaultValue;
						continue;
					}
					
					string arg = args[i];
					var value = ParseType( caller, type, arg );
					if(value == null)
					{
						Logging.Error( $"Tried to execute command {command} but the argument {arg} couldn't be converted to {type}!" );
						return;
					}

					parameterValues[i] = value;
				}

				Debug.Log( $"Executing {command} with parameters" );
				foreach(var p in parameterValues)
				{
					Debug.Log( $"- {p}" );
				}

				Context = new( caller );
				method.Invoke( null, parameterValues );
			}
			else
			{
				Debug.Log( $"Executing {command} without parameters" );

				Context = new( caller );
				method.Invoke( null, null );
			}
		}

		private static object ParseType( IClient caller, Type type, string arg)
		{
			if ( type.IsAssignableFrom( arg.GetType() ) )
			{
				return arg;
			}

			object value = null;
			try
			{
				value = Convert.ChangeType( arg, type );
				Debug.Log( $"Converted {arg} to type {type}" );
			}
			catch ( InvalidCastException )
			{
				// We cant cast with the builtin parsers, try custom ones

				bool parsed = false;

				var parsers = GetParsers( type );
				var parserCount = parsers?.Count();
				if ( parsers == null || parserCount == 0 )
				{
					return null;
				}

				Debug.Log( $"Found {parserCount} parsers for type {type}!" );
				foreach ( var parser in parsers )
				{
					Debug.Log( $"Trying out parser {parser}" );
					value = parser.Parse( caller, arg );
					if ( value != default && type.IsAssignableFrom( value.GetType() ) )
					{
						Debug.Log( $"Successfully parsed {arg} to {value}!" );
						parsed = true;
						break;
					}
				}

				if ( !parsed )
				{
					return null;
				}
			}

			return value;
		}
		private static IEnumerable<ICommandParser<T>> GetParsers<T>()
		{
			return TypeLibrary.GetTypes<ICommandParser<T>>()
								.Select(t => t.Create<ICommandParser<T>>());
		}
		
		private static IEnumerable<ICommandParser> GetParsers(Type t)
		{
			return TypeLibrary.GetTypes()
									   .Where( td =>
										   td.Interfaces.Any(
											   i => i.IsAssignableTo(typeof( ICommandParser ) )
												   && i.FullName.Contains( t.Name )
										   )
									   )
									   .Select(td => td.Create<ICommandParser>());
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
		public string Description { get; set; }
		public CommandAttribute(string name)
		{
			Name = name;
		}
	}
}
