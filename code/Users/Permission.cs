using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Breaker
{
	public static class Permission
	{
		public static bool Has( IClient cl, string permission )
		{
			if ( cl.IsListenServerHost )
				return true;

			return Has( User.Get( cl ), permission );
		}
		public static bool Has( User user, string permission )
		{
			var permissions = user.GetPermissions();

			return permissions.Contains( permission );
		}
	}
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
	public class PermissionAttribute : Attribute
	{
		public string Permission { get; private set; }
		/// <summary>
		/// Dont check for this permission when executing the command
		/// </summary>
		public bool ManualEnforcement { get; init; }
		public PermissionAttribute( string permission, bool manualEnforcement = false )
		{
			Permission = permission;
			ManualEnforcement = manualEnforcement;
		}

		public static implicit operator string( PermissionAttribute attr ) => attr.Permission;
	}
}
