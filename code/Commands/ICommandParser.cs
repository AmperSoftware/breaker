using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	/// <summary>
	/// Parses input into object of specified type
	/// </summary>
	/// <typeparam name="T">Type to parse to object into</typeparam>
	public interface ICommandParser<T> : ICommandParser
	{
		public new T Parse( IClient caller, string input );
		public new IEnumerable<T> ParseMultiple( IClient caller, string input )
		{
			var elements = new List<T>();
			foreach ( var part in input.Split( ',' ) )
			{
				var parsed = Parse( caller, part );
				if ( parsed != null )
					elements.Add( parsed );
			}

			return elements;
		}
	}
	
	/// <summary>
	/// Parses input into some object
	/// </summary>
	public interface ICommandParser
	{
		public object Parse( IClient caller, string input );
		public virtual IEnumerable ParseMultiple( IClient caller, string input )
		{
			var elements = new List<object>();
			foreach ( var part in input.Split( ',' ) )
			{
				var parsed = Parse( caller, part );
				if ( parsed != null )
					elements.Add( parsed );
			}

			return elements;
		}
	}
}
