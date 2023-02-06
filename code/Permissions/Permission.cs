using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using static Sandbox.Event;

namespace Breaker
{
	public static class Permission
	{
		private static Dictionary<IClient, List<string>> clientPermissions = new();
		public static IEnumerable<string> GetPermissions( IClient client )
		{
			return new string[] { "breaker.menu" };
		}
		public static bool Has(IClient cl, string permission)
		{
			var permissions = GetPermissions( cl );
			if ( permissions.Contains( permission ) )
				return true;

			return false;
		}
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class PermissionAttribute : Attribute
	{
		public string Permission { get; private set; }
		public PermissionAttribute(string permission)
		{
			Permission = permission;
		}

		public static implicit operator string( PermissionAttribute attr ) => attr.Permission;
	}
}
