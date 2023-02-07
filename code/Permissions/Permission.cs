using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using static Sandbox.Event;

namespace Breaker
{
	public static class Permission
	{
		public static bool Has(IClient cl, string permission)
		{
			if ( cl.IsListenServerHost )
				return true;

			return Has( User.Get( cl ), permission );
		}
		public static bool Has(User user, string permission)
		{
			var permissions = user.GetPermissions();

			return permissions.Contains( permission );
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
