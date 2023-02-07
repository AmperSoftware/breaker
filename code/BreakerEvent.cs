using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class BKREvent
	{
		public class ConfigLoadedAttribute : EventAttribute
		{
			public ConfigLoadedAttribute() : base( "breaker.config.loaded" ) { }
		}
	}
}
