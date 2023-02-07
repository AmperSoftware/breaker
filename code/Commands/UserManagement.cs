using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	public static class UserManagement
	{
		[Command("userprint")]
		public static void PrintUsers()
		{
			foreach(var cl in Game.Clients)
			{
				var user = User.Get( cl );
				Util.LogInfo( $"{cl.Name} | Groups: ({string.Join(",", user.UserGroups)})" );
			}
		}

		[Command("usergroup"),Permission("breaker.user.group")]
		public static void SetGroup(IClient target, string group)
		{
			var user = User.Get( target );
			if(UserGroup.Exists(group))
			{
				user.UserGroups.Add( group );
				User.Update( user );
				Debug.Log( $"Added client {target} to group {group}" );
			}
			else
			{
				Debug.Log( $"Group {group} does not exist!" );
			}
		}
	}
}
