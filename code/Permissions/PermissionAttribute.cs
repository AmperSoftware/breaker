using System;
using Sandbox;

namespace Breaker
{
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
