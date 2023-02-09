using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static partial class Logging
	{
		[Command("tell")]
		public static void TellClient( IClient client, string message )
		{
			if ( client == null ) return;

			TellClientRPC( To.Single( client ), message );
		}
		
		[ClientRpc]
		internal static void TellClientRPC( string message )
		{
			Info( message );
			return;
		}
		public static void Info(object message)
		{
			Log.Info( $"[Breaker] {message}" );
		}

		public static void Error(object message)
		{
			Log.Error( $"[Breaker] {message}" );
		}
	}
}
