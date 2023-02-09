﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	[Category("Moderation")]
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

			public string GetDurationString()
			{
				if ( IsPermanent )
					return "Permanent";

				const int SECONDS_IN_DAY = 86400;
				const int SECONDS_IN_HOUR = 3600;
				const int SECONDS_IN_MINUTE = 60;

				var time = TimeSpan.FromSeconds( Duration );

				if (Duration > SECONDS_IN_DAY)
					return $"{time.TotalDays}d";
				else if(Duration > SECONDS_IN_HOUR )
					return $"{time.TotalHours}h";
				else if(Duration > SECONDS_IN_MINUTE )
					return $"{time.TotalMinutes}m";
				else
					return $"{time.TotalSeconds}s";
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
			}
				Logging.TellAll( $"{Command.Caller} kicked {Logging.FormatClients(targets)} for {reason}" );
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
					Logging.TellAll( $"{Command.Caller} banned {Logging.FormatClients(targets)} permanently for {reason}." );
				else
					Logging.TellAll( $"{Command.Caller} banned {Logging.FormatClients(targets)} for {entry.GetDurationString()} for {reason}." );
			}
			SaveBans();
		}

		[Command( "unban" ), Permission( "breaker.unban" )]
		public static void Unban( IEnumerable<IClient> targets )
		{
			foreach ( var target in targets )
			{
				bans.RemoveAll( b => b.SteamId == target.SteamId );
			}
			SaveBans();
			Logging.TellAll( $"{Command.Caller} unbanned {Logging.FormatClients( targets )}." );
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
