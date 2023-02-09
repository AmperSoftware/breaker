using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	[Category("User Management")]
	public static class UserManagement
	{
		[Command("getusers")]
		public static void PrintUsers()
		{
			Logging.TellClient( Command.Context.Caller, "Groups:" );
			foreach (var cl in Game.Clients)
			{
				var user = User.Get( cl );
				Logging.TellClient( Command.Context.Caller, $"{cl.Name} | Groups: ({string.Join(",", user.UserGroups)})" );
			}
		}

		[Command("usergroup"),Permission("breaker.user.group")]
		public static void ManageGroup([Title("add/remove")] string mode, IClient target, string group)
		{
			var user = User.Get( target );
			switch(mode)
			{
				case "add":
					AddGroup( user, group );
					Logging.Info( $"Added client {target} to group {group}" );
					break;
				case "remove":
					RemoveGroup( user, group );
					Logging.Info( $"Removed client {target} from group {group}" );
					break;
				default:
					Logging.Error( $"Invalid mode {mode}!" );
					break;
			}
		}

		private static void AddGroup(User user, string group)
		{
			if ( UserGroup.Exists( group ) )
			{
				if ( user.UserGroups.Contains( group ) )
				{
					Logging.Error( $"Client is already in group {group}!" );
					return;
				}
				user.UserGroups.Add( group );
				User.Update( user );
			}
			else
			{
				Logging.Error( $"Group {group} does not exist!" );
			}
		}

		private static void RemoveGroup(User user, string group)
		{
			if ( user.UserGroups.Contains( group ) )
			{
				user.UserGroups.Remove( group );
				User.Update( user );
			}
			else
			{
				Logging.Error( $"Client is not in group {group}!" );
			}
		}
	}
}
