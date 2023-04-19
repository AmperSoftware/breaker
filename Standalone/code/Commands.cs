using Breaker.Addon.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Addon;

public static class Commands
{
	[Command( "menu" ), Permission( "breaker.menu" )]
	public static void OpenMenu()
	{
		Debug.Log( "Opening Menu..." );
		BreakerMenu.Show( To.Single( Command.Caller ) );
	}

	[Command( "slap" ), Permission( "breaker.slap" )]
	public static void Slap( IClient[] targets, float force, float damage = 1 )
	{
		foreach ( var target in targets )
		{

			var pawn = target.Pawn;
			var speed = Vector3.Random * force;
			if ( speed.z <= force / 4 )
			{
				speed = speed.WithZ( force / 4 );
			}

			Debug.Log( $"Slapping target {target} with force {force} ({speed})" );

			if ( pawn is ModelEntity ent && ent.PhysicsEnabled )
			{
				DamageInfo dmg = DamageInfo.Generic( damage )
											.WithForce( speed / 10 );

				// Force the player to be at least a bit above the ground
				// pawn.Position = pawn.Position.WithZ( pawn.Position.z + 10 );
				// Then apply the velocity because not all gamemodes respect DamageInfo.Force
				pawn.Velocity = pawn.Velocity += speed;

				ent.TakeDamage( dmg );
				ent.PlaySound( "slap" );
			}
		}
		Logging.TellAll( $"{Command.CallerName} slapped {Logging.FormatClients( targets )}!" );
	}
}
