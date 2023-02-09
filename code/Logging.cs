using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public static partial class Logging
	{
		public static void TellClient( IClient client, string message, MessageType type = MessageType.Info )
		{
			if ( client == null ) return;

			TellClientRPC( To.Single( client ), message, type );
		}
		public static void TellCaller(string message, MessageType type = MessageType.Info )
		{
			TellClient( Command.Caller, message, type );
		}
		
		[ClientRpc]
		internal static void TellClientRPC( string message, MessageType type )
		{
			switch(type)
			{
				case MessageType.Info:
					Info( message );
					break;
				case MessageType.Error:
					Error( message );
					break;
				case MessageType.Announcement:
					// TODO: Make this fancier
					Info( $"[ANNOUNCEMENT] {message}" );
					break;
			}
		}
		public static void Info(object message)
		{
			Log.Info( $"[Breaker] {message}" );
		}

		public static void Error(object message)
		{
			Log.Error( $"[Breaker] {message}" );
		}
	}
	public enum MessageType
	{
		Info,
		Error,
		Announcement
	}
}
