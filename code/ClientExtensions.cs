using Breaker;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class ClientExtensions
    {
		public static bool HasPermission( this IClient client, string permission )
		{
			return Permission.Has( client, permission );
		}
		public static void ExecuteCommand(this IClient client, string command, string[] args)
		{
			Command.Execute( command, client, args );
		}
	}
}
