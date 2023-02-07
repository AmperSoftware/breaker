using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class Logging
	{
		public static void TellClient( IClient client, string message )
		{
			if ( client == null ) return;
			Sandbox.UI.ChatBox.AddInformation( To.Single(client), message );
		}
		public static void LogInfo(object message)
		{
			Log.Info( $"[Breaker] {message}" );
		}

		public static void LogError(object message)
		{
			Log.Error( $"[Breaker] {message}" );
		}
	}
}
