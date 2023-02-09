using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	public static class GroupManagement
	{
		[Command( "getgroups" )]
		public static void PrintGroups()
		{
			Logging.TellClient( Command.Context.Caller, "Groups:" );
			foreach ( var kv in UserGroup.All )
			{
				Logging.TellClient( Command.Context.Caller, $"- {kv.Key} ({kv.Value.Weight}) [{string.Join(", ",kv.Value.Permissions)}]" );
			}
		}
	}
}
