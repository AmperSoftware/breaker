using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	[Category( "User Groups" )]
	public static class GroupManagement
	{
		[Command( "getgroups" )]
		public static void Print()
		{
			Logging.TellCaller(  "Groups:" );
			foreach ( var kv in UserGroup.All )
			{
				Logging.TellCaller( $"- {kv.Key} ({kv.Value.Weight}) [{string.Join(", ",kv.Value.Permissions)}]" );
			}
		}

		[Command("groupadd"), Permission("breaker.group.create")]
		public static void Add(string id, int weight, IEnumerable<string> permissions = default)
		{
			UserGroup group = new( id, weight, permissions?.ToList() );

			if ( UserGroup.Exists( id ) )
			{
				Logging.TellCaller( $"Group {id} already exists!", MessageType.Error );
				return;
			}

			UserGroup.Add( group );
			Logging.TellCaller( $"Created group {id} with weight {weight} and {permissions.Count()} permissions." );
		}

		[Command("groupremove"), Permission("breaker.group.remove")]
		public static void Remove(string id)
		{
			if ( !UserGroup.Exists( id ) )
			{
				Logging.TellCaller( $"Group {id} does not exist!", MessageType.Error );
				return;
			}

			UserGroup.Remove( id );
			Logging.TellCaller( $"Removed group {id}." );
		}

		[Command("groupperms"), Permission("breaker.group.edit.permissions")]
		public static void EditPermissions([Title("add/remove")]string action, string id, string permission)
		{
			if ( !UserGroup.Exists( id ) )
			{
				Logging.TellCaller( $"Group {id} does not exist!", MessageType.Error );
				return;
			}

			var group = UserGroup.All[id];
			switch(action)
			{
				case "add":
					if ( group.Permissions.Contains( permission ) )
					{
						Logging.TellCaller( $"Group {id} already has permission {permission}!", MessageType.Error );
						return;
					}
					group.Permissions.Add( permission );
					UserGroup.Update( group );
					Logging.TellCaller( $"Added permission {permission} to group {id}." );
					break;
				case "remove":
					if ( !group.Permissions.Contains( permission ) )
					{
						Logging.TellCaller( $"Group {id} does not have permission {permission}!", MessageType.Error );
						return;
					}
					group.Permissions.Remove( permission );
					UserGroup.Update( group );
					Logging.TellCaller( $"Removed permission {permission} from group {id}." );
					break;
				default:
					Logging.TellCaller( $"Invalid action {action}!", MessageType.Error );
					break;
			}
		}
		[Command("groupweight"), Permission("breaker.group.edit.weight")]
		public static void EditWeight(string id, int weight)
		{
			if ( !UserGroup.Exists( id ) )
			{
				Logging.TellCaller( $"Group {id} does not exist!", MessageType.Error );
				return;
			}

			var group = UserGroup.All[id];
			group.Weight = weight;
			UserGroup.Update( group );
			Logging.TellCaller( $"Set weight of group {id} to {weight}." );
		}

		[Command("groupsettings"), Permission("breaker.group.edit.settings")]
		public static void EditSettings(string id, string setting, string value)
		{
			if ( !UserGroup.Exists( id ) )
			{
				Logging.TellCaller( $"Group {id} does not exist!", MessageType.Error );
				return;
			}

			var group = UserGroup.All[id];
			switch ( setting )
			{
				case "name":
					group.Name = value;
					break;
				case "namecolor":
					group.NameColor = value;
					break;
				case "prefix":
					group.Prefix = value;
					break;
				case "prefixcolor":
					group.PrefixColor = value;
					break;
				case "icon":
					group.Icon = value;
					break;
				default:
					Logging.TellCaller( $"Invalid setting {setting}!", MessageType.Error );
					return;
			}
			UserGroup.Update( group );
			Logging.TellCaller( $"Set {setting} of group {id} to {value}." );
		}
	}
}
