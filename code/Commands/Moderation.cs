using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	public static class Moderation
	{
		#region Moderation Persistence
		private const string BAN_FILE = "bans.json";
		private static BaseFileSystem fs => Config.Instance.GetFileSystem();
		private static List<BanEntry> bans = new();
		public struct BanEntry
		{
			public long SteamId { get; set; }
			public string Reason { get; set; }
			public DateTime StartTime { get; set; }
			public float Duration { get; set; }
			public bool IsPermanent => Duration < 0;
			public DateTime GetEndTime()
			{
				return StartTime.AddSeconds( Duration );
			}
		}

		public static void LoadBans()
		{
			bans = fs.ReadJsonOrDefault<List<BanEntry>>( BAN_FILE );
		}
		public static void SaveBans()
		{
			fs.WriteJson( BAN_FILE, bans );
		}
		#endregion
		[Command("kick"), Permission("breaker.kick")]
		public static void Kick( IEnumerable<IClient> targets, string reason = "No reason given." )
		{
			foreach ( var target in targets )
			{
				target.Kick();
				Util.LogInfo( $"Kicked {target} for {reason}" );
			}
		}

		[Command( "ban" ), Permission( "breaker.ban" )]
		public static void Ban( IEnumerable<IClient> targets, float duration = -1, string reason = "No reason given." )
		{
			foreach ( var target in targets )
			{
				BanEntry entry = new()
				{
					SteamId = target.SteamId,
					Reason = reason,
					StartTime = DateTime.Now,
					Duration = duration
				};
				bans.Add( entry );
				target.Kick();
				if(entry.IsPermanent)
					Util.LogInfo( $"Permanently banned {target} for {reason}." );
				else
					Util.LogInfo( $"Banned {target} for {reason} for {duration} seconds." );
			}
			SaveBans();
		}

		[Command( "unban" ), Permission( "breaker.unban" )]
		public static void Unban( IEnumerable<IClient> targets )
		{
			foreach ( var target in targets )
			{
				bans.RemoveAll( b => b.SteamId == target.SteamId );
				Util.LogInfo( $"Unbanned {target}." );
			}
			SaveBans();
		}

		public static bool IsBanned(IClient cl)
		{
			return bans.Any( b => b.SteamId == cl.SteamId && (b.IsPermanent || b.GetEndTime() > DateTime.Now) );
		}

		[BRKEvent.PlayerJoined]
		public static void ClientJoined(IClient cl)
		{
			if(IsBanned(cl))
				cl.Kick();
		}
	}
}
