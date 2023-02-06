using Sandbox;
using Sandbox.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Util
{
	public static class Debug
	{
		[ConVar.Replicated] public static bool breaker_debug { get; set; } = false;
		public static bool Enabled()
		{
			return breaker_debug;
		}

		public static void Log(string message)
		{
			if ( !Enabled() )
				return;

			GlobalGameNamespace.Log.Info($"[Breaker] {message}");
		}
		public static void Log( object message )
		{
			Log( message.ToString() );
		}
	}
}
