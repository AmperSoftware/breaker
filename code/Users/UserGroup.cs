using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
    public class UserGroup
    {
		#region Persistent User Groups
		const string GROUP_FILE = "breaker/groups.json";
		private static BaseFileSystem fs => Config.Instance.GetFileSystem();
		private static Dictionary<string, UserGroup> groups = new();
		public static IReadOnlyDictionary<string,UserGroup> All => groups.AsReadOnly();
		private static readonly Dictionary<string, UserGroup> defaultGroups = new()
		{
			{ "user", new() {Id = "user", Weight = 0 } },
			{ "admin", new() {Id = "admin", Weight = 100} }
		};
		public static void Load()
		{
			groups = fs.ReadJsonOrDefault( GROUP_FILE, defaultGroups );
		}

		public static void Save()
		{
			if ( groups.Count == 0 )
				groups = defaultGroups;

			fs.WriteJson( GROUP_FILE, groups );
		}

		public static void Add(UserGroup group)
		{
			if(groups.ContainsKey(group.Id))
			{
				Util.LogError( $"Tried to register group with duplicate id {group.Id}!" );
				return;
			}

			groups.Add( group.Id, group );

			Save();
		}

		public static void Remove(string id)
		{
			if ( !groups.ContainsKey( id ) )
			{
				Util.LogError( $"Tried to remove group with id {id} which doesnt exist!" );
				return;
			}

			groups.Remove( id );

			Save();
		}

		public static void Remove( UserGroup group ) => Remove( group.Id );
		#endregion
		public static UserGroup GetDefault()
		{
			if ( !string.IsNullOrEmpty( Config.Instance?.DefaultUserGroup ) )
			{
				if ( UserGroup.All.TryGetValue( Config.Instance.DefaultUserGroup, out var group ) )
					return group;
			}

			return UserGroup.All.Select( kv => kv.Value ).OrderBy( g => g.Weight ).First();
		}
		public UserGroup()
		{
			// Required for JSON
		}

		public UserGroup( string id, int weight, List<string> permissions )
		{
			Id = id;
			Weight = weight;
			Permissions = permissions;
		}

		public string Id { get; set; }
		
		#region Settings
		public string Name { get; set; }
		public Color NameColor { get; set; }
		public string Prefix { get; set; }
		public Color PrefixColor { get; set; }
		/// <summary>
		/// The icon to use for this group. This is a URL or Path to an image.
		/// </summary>
		public string Icon { get; set; }
		#endregion Settings

		/// <summary>
		/// Decides what users may be targeted by members of this group.
		/// Users can always target other Users with a weight lower then their own.
		/// </summary>
		public int Weight { get; set; }
		public List<string> Permissions { get; set; } = new();

		public bool CanTarget( UserGroup target, string command )
		{
			return target.Weight < Weight;
		}
	}
}
