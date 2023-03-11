using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.UI
{
	public interface ICommandParameterPanel
	{
		public string TypeName { get; }
		public void Initialize( string value );
		public string GetValue();
	}
}
