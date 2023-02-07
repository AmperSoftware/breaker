﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static class ReservedSlots
	{
		private const string RESERVED_SLOT_PERMISSION = "breaker.useslot";
		private static int publicSlots => Game.Server.MaxPlayers - Console.Vars.breaker_reserved_slots;
		private static int currentPlayers => Game.Clients.Count();
		public static bool Enabled()
		{
			return Console.Vars.breaker_reserved_slots > 0;
		}

		[BRKEvent.PlayerJoined]
		private static void PlayerJoined(IClient cl)
		{
			if ( currentPlayers > publicSlots && !cl.HasPermission( RESERVED_SLOT_PERMISSION ) )
			{
				cl.Kick();
			}
		}
	}
}
