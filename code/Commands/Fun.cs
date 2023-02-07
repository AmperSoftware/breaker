using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.Commands
{
	public static class Fun
	{
		[Command( "slay" ), Permission( "breaker.slay" )]
		public static void Slay(IEnumerable<IClient> targets)
		{
			foreach(var target in targets)
			{
				var pawn = target.Pawn;
				if ( pawn is Entity ent )
					ent.Health = 0;
			}
		}
	}
}
