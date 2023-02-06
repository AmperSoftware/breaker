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
		public static IEnumerable<string> GetPermissions( this IClient client )
		{
			return new string[] { "breaker.menu" };
		}
		public static bool HasPermission( this IClient client, string permission )
		{
			var permissions = GetPermissions( client );
			if ( permissions.Contains( permission ) )
				return true;
			
			return client.IsListenServerHost;
		}
	}
}
