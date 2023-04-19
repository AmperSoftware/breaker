using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public class DefaultChatLogger : Logging.ILogger
	{
		[BRKEvent.ConfigLoaded]
		static void OnConfigLoaded()
		{
			DefaultChatLogger instance = new();
			Logging.RegisterLogger( instance );
		}
		public bool Server => false;

		public bool Client => true;

		public void Log( string message, MessageType type = MessageType.Info )
		{
			if ( type == MessageType.Error )
				message = "[ERROR] " + message;
			else if ( type == MessageType.Announcement )
				message = "[ANNOUNCEMENT] " + message;

			//ChatBox.AddInformation( message, AddonInformation.ICON );
			ConsoleSystem.Run( "chat_addinfo", message, AddonInformation.ICON );
		}
	}
}
