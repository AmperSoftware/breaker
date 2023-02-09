using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.UI
{
	public partial class BreakerMenu : Panel
	{
		public void OnClickExit()
		{
			SetClass( "visible", false );
		}
	}
}
