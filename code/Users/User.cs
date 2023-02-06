using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	/// <summary>
	/// Class for handling user information and persistent storage.
	/// </summary>
	public class User
	{
		#region Persistent Users
		const string USER_FILE = "breaker/users.json";
		private static BaseFileSystem fs => Config.Instance.GetFileSystem();
		private static Dictionary<long, User> users = new();
		public static IReadOnlyDictionary<long, User> All => users.AsReadOnly();
		public static User Get( IClient client ) => users[client.SteamId];
		public static void Load()
		{
			users = fs.ReadJsonOrDefault( USER_FILE, new Dictionary<long, User>() );
		}
		public static void Save()
		{
			if ( users.Count == 0 )
				users = new();
			
			fs.WriteJson( USER_FILE, users );
		}
		public static void Add( User user )
		{
			if ( users.ContainsKey( user.SteamID ) )
			{
				Util.LogError( $"Tried to register user with duplicate id {user.SteamID}! " );
				return;
			}
			users.Add( user.SteamID, user );
			Save();
		}
		public static void Add(IClient client)
		{
			User user = new() { 
				SteamID = client.SteamId, 
				UserGroups = new() { UserGroup.GetDefault().Id } 
			};
			
			Add( user );
		}
		public static void Update( User user )
		{
			if ( !users.ContainsKey( user.SteamID ))
			{
				Util.LogError( $"Tried to update user with id {user.SteamID} which doesnt exist!" );
				return;
			}
			users[user.SteamID] = user;
			Save();
		}
		#endregion
		public long SteamID { get; set; }
		/// <summary>
		/// User groups by id.
		/// </summary>
		public List<string> UserGroups { get; set; } = new();
		/// <summary>
		/// User-specific permissions
		/// </summary>
		public List<string> Permissions { get; set; } = new();
		public IEnumerable<UserGroup> GetUserGroupInfo()
		{
			return UserGroups.Select( id => UserGroup.All[id] );
		}
		
		public List<string> GetPermissions()
		{
			var perms = new List<string>();
			foreach ( var group in GetUserGroupInfo() )
			{
				perms.AddRange( group.Permissions );
			}
			perms.AddRange( Permissions );
			return perms;
		}

		public bool CanTarget( User target, string command )
		{
			var targetGroups = target.GetUserGroupInfo();
			foreach ( var group in GetUserGroupInfo() )
			{
				foreach ( var targetGroup in targetGroups )
				{
					if ( group.CanTarget( targetGroup, command ) )
						return true;
				}
			}
			return false;
		}
	}
}
