using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	[Category("Fun")]
	public static class Fun
	{
		[Command( "slay" ), Permission( "breaker.slay" )]
		public static void Slay(IEnumerable<IClient> targets)
		{
			Debug.Log( $"Slaying {targets.Count()} targets" );
			foreach (var target in targets)
			{
				Debug.Log( $"Slaying target {target}" );

				var pawn = target.Pawn;
				if ( pawn is Entity ent )
				{
					ent.Health = 0;
					ent.OnKilled();
				}
			}
		}
	}
}
