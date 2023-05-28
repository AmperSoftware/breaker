﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	[Category( "Fun" )]
	public static class Fun
	{
		[Command( "slay" ), Permission( "breaker.slay" )]
		public static void Slay( IClient[] targets )
		{
			foreach ( var target in targets )
			{
				Debug.Log( $"Slaying target {target}" );

				var pawn = target.Pawn;
				if ( pawn is Entity ent )
				{
					ent.TakeDamage( DamageInfo.Generic( ent.Health * 2 ) );
					if(ent.LifeState == LifeState.Alive)
					{
						ent.OnKilled();
					}
				}
			}
			Logging.TellAll( $"{Command.CallerName} slayed {Logging.FormatClients( targets )}!" );
		}

		[Command("launch"), Permission("breaker.launch")]
		public static void Launch( IClient[] targets, float force = 768f)
		{
			foreach ( var target in targets )
			{
				var pawn = target.Pawn;
				var speed = Vector3.Up * force;

				if ( pawn is ModelEntity ent && ent.PhysicsEnabled )
				{
					pawn.Velocity = pawn.Velocity += speed;
				}
			}
			Logging.TellAll( $"{Command.CallerName} slapped {Logging.FormatClients( targets )}!" );
		}
	}
}
