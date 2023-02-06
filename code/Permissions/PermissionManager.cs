using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	internal static class PermissionManager
	{
		static Dictionary<PermissionAttribute, MethodDescription> guardedMethods = new();
		[Event.Entity.PostSpawn]
		static void OnLoad()
		{
			var methods = Attributes.FindMethods<PermissionAttribute>();
			foreach ( var method in methods )
			{
				method.
			}
		}
	}
}
