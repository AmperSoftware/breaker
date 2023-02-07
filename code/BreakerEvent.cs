using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class BRKEvent
	{
		public class ConfigLoadedAttribute : EventAttribute
		{
			public ConfigLoadedAttribute() : base( "breaker.config.loaded" ) { }
		}
		public class PlayerNumberChangedAttribute : EventAttribute
		{
			// TODO: This is stupid, get a way to only check for this if a client joined
			public PlayerNumberChangedAttribute() : base( "breaker.players.changed" ) { }
		}
		public class PlayerJoinedAttribute : EventAttribute
		{
			public PlayerJoinedAttribute() : base("breaker.player.joined") {  }
		}

		static int lastPlayerCount = 0;
		static List<long> joinedPlayers = new();
		[Event.Tick.Server]
		private static void Tick()
		{
			var clients = Game.Clients;
			if ( clients == null || clients.Count() == lastPlayerCount )
				return;

			foreach(var client in clients)
			{
				if ( !joinedPlayers.Contains( client.SteamId ) )
				{
					joinedPlayers.Add( client.SteamId );
					Event.Run( "Breaker.player.joined", client );
				}
			}
			
			lastPlayerCount = clients.Count();
			Event.Run( "Breaker.players.changed" );
		}
	}
}
