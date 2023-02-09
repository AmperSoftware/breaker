﻿using Breaker.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	[Category("Basic")]
	public static class Console
	{
		/// <summary>
		/// All the basic breaker commands.
		/// </summary>
		public static class Commands
		{
			[ConCmd.Server( "brk" )]
			public static void RunCommand(string command, string p1 = "", string p2 = "", string p3 = "", string p4 = "", string p5 = "", string p6 = "" )
			{
				var parameters = new string[] { p1, p2, p3, p4, p5, p6 };
				Command.Execute( command, ConsoleSystem.Caller, parameters.Where( p => !string.IsNullOrEmpty( p ) ).ToArray() );
			}
			[Command( "menu" ), Permission( "breaker.menu" )]
			public static void OpenMenu()
			{
				Debug.Log( "Opening Menu..." );
				BreakerMenu.Show(To.Single(Command.Caller));
			}
			[Command("reload"), Permission("breaker.reload")]
			public static void Reload()
			{
				Logging.TellCaller( "Reloading..." );
				Config.Load();
				
				Logging.TellCaller( "Finished Reloading!");
				Logging.TellCaller( $"{Command.All.Count} commands loaded" );
				Logging.TellCaller( $"{User.All.Count} users loaded" );
				Logging.TellCaller( $"{UserGroup.All.Count} user groups loaded" );
			}

			const int COMMANDS_PER_PAGE = 10;
			[Command( "help" )]
			public static void Help(int page = 1)
			{
				if ( page < 1 )
					page = 1;

				var cmds = Command.All.OrderBy(kv => kv.Key).ToArray();
				int length = cmds.Length;
				int pageCount = MathX.CeilToInt(length / (float)COMMANDS_PER_PAGE);
				if(pageCount < 1)
					pageCount = 1;

				Logging.TellCaller( $"{length} commands currently registered." );
				Logging.TellCaller( $"Page {page} of {pageCount}" );
				for ( int i = COMMANDS_PER_PAGE * (page-1); i < length; i++ )
				{
					PrintCommand( cmds[i] );
				}
			}

			private static void PrintCommand( KeyValuePair<string, Command.CommandInfo> cmd )
			{
				string name = cmd.Key;
				var p = Command.Parameters( name );
				if ( p.Length > 0 )
				{
					name += $" ({string.Join( ", ", p.Select( x => $"{PrettyParamName(x)}: {PrettyTypeName(x.ParameterType)}" ) )})";
				}
				string desc = cmd.Value.Method.Description;
				if ( !string.IsNullOrEmpty( desc ) )
					name += $"| {desc}";
				
				Logging.TellCaller( $"- {name}" );
			}
			private static string PrettyParamName(ParameterInfo p)
			{
				var title = p.GetCustomAttribute<TitleAttribute>();
				if ( title != null )
					return title.Value;

				return p.Name;
			}
			private static string PrettyTypeName(Type t)
			{
				if ( t == typeof( string ) )
					return "String";
				if ( t == typeof( int ) )
					return "Integer";
				if ( t == typeof( float ) )
					return "Float";
				if ( t == typeof( bool ) )
					return "Boolean";
				if ( t == typeof( Vector3 ) )
					return "Vector";
				if ( t == typeof( IClient ) )
					return "Client";
				if ( t == typeof( IEnumerable<IClient> ) )
					return "Clients";
				return t.Name;
			}
		}

		/// <summary>
		/// All the basic breaker console variables.
		/// </summary>
		public static class Vars
		{
			[ConVar.Replicated] public static int breaker_reserved_slots { get; set; } = 0;
			[ConVar.Replicated] public static bool breaker_whitelist_enabled { get; set; } = false;
			[ConVar.Replicated] public static bool breaker_debug { get; set; } = false;
		}
	}
}
