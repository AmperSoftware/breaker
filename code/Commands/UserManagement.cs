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
		public static void AddGroup(IClient target, string group)
		{
			var user = User.Get( target );
			if(UserGroup.Exists(group))
			{
				user.UserGroups.Add( group );
				User.Update( user );
				Util.LogInfo( $"Added client {target} to group {group}" );
			}
			else
			{
				Util.LogError( $"Group {group} does not exist!" );
			}
		}

		[Command( "usergroupremove" ), Permission( "breaker.user.group" )]
		public static void RemoveGroup(IClient target, string group)
		{
			var user = User.Get( target );
			if(user.UserGroups.Contains(group))
			{
				user.UserGroups.Remove( group );
				User.Update( user );
				Util.LogInfo( $"Removed client {target} from group {group}" );
			}
			else
			{
				Util.LogError( $"Client is not in group {group}!" );
			}
		}
	}
}
