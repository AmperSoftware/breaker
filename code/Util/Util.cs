using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class Util
	{
		public static void LogInfo(object message)
		{
			Log.Info( $"[Breaker] {message}" );
		}
	}
}
