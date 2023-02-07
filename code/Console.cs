using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class Console
	{
		/// <summary>
		/// All the basic breaker commands.
		/// </summary>
		public static class Commands
		{
			[ConCmd.Server( "bkr" )]
			public static void RunCommand(string command, string p1 = "", string p2 = "", string p3 = "", string p4 = "" )
			{
				var parameters = new string[] { p1, p2, p3, p4 };
				Command.Execute( command, ConsoleSystem.Caller, parameters.Where( p => !string.IsNullOrEmpty( p ) ).ToArray() );
			}
			[Command( "menu" ), Permission( "breaker.menu" )]
			public static void OpenMenu()
			{
				Debug.Log( "Opening Menu" );
			}
			[Command("reload"), Permission("breaker.reload")]
			public static void Reload()
			{
				Util.LogInfo( "Reloading..." );
				Config.Load();
				
				Util.LogInfo("Finished Reloading!");
				Util.LogInfo( $"{Command.All.Count} commands loaded" );
				Util.LogInfo( $"{User.All.Count} users loaded" );
				Util.LogInfo( $"{UserGroup.All.Count} user groups loaded" );
			}

			const int COMMANDS_PER_PAGE = 10;
			[Command( "help" )]
			public static void Help(int page = 1)
			{
				if ( page < 1 )
					page = 1;

				var cmds = Command.All.ToArray();
				int length = cmds.Length;
				int pageCount = length / COMMANDS_PER_PAGE;
				if(pageCount < 1)
					pageCount = 1;

				Util.LogInfo( $"{length} commands currently registered." );
				Util.LogInfo( $"Page {page} of {pageCount}" );
				for ( int i = COMMANDS_PER_PAGE * (page-1); i < length; i++ )
				{
					var cmd = cmds[i];
					string name = cmd.Key;
					var p = Command.Parameters( name );
					if ( p.Length > 0 )
					{
						name += $" ({string.Join( " ,", p.Select( x => $"{x.Name}: {x.ParameterType.Name}" ) )})";
					}
					Util.LogInfo( $"- {name}" );
				}
			}
		}

		/// <summary>
		/// All the basic breaker console variables.
		/// </summary>
		public static class Vars
		{
			[ConVar.Replicated] public static bool breaker_debug { get; set; } = false;
		}
	}
}
