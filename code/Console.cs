using Breaker.Util;
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
		public static class Commands
		{
			[ConCmd.Server( "breaker" )]
			public static void RunCommand(string command, string[] parameters)
			{
				Command.Execute( command, ConsoleSystem.Caller, parameters );
			}
			[Command( "breaker_menu" ), Permission( "breaker.menu" )]
			public static void OpenMenu()
			{
				Debug.Log( "Opening Menu" );
			}
			
			[ConCmd.Server( "breaker_help" )]
			public static void Help()
			{
				var cmds = Command.All;

				Log.Info( $"{cmds.Count} commands currently registered:" );
				foreach ( var cmd in cmds )
				{
					Log.Info( $"- {cmd.Key}" );
				}
			}
		}
	}
}
