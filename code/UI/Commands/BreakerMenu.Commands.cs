using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker.UI
{
	public partial class BreakerMenu
	{
		private class ParameterPanels
		{
			public Panel Root;
			public TextEntry Input;

			public ParameterPanels( Panel root, TextEntry input )
			{
				Root = root;
				Input = input;
			}
		}
		internal static IEnumerable<IGrouping<string, Command.ClientInfo>> groups => Command.GetAllClientGrouped();
		private Dictionary<string, List<Panel>> groupPanels = new();
		private Dictionary<string, ParameterPanels> parameterEntries = new();
		Panel ListPanel { get; set; }
		bool createdPanels = false;
		Label CommandTitleLabel { get; set; }
		Panel ParameterPanel { get; set; }
		public void OnClickExit()
		{
			Debug.Log( $"Exiting menu..." );
			Hide();
		}
		public void OnClickCommand( Command.ClientInfo cmd )
		{
			Debug.Log( $"Clicked on command {cmd.Key}" );
			SetupCommandWizard( cmd );
		}

		public void OnClickGroup( string group )
		{
			Debug.Log( $"Clicked on group {group}" );
			if(!groupPanels.TryGetValue(group, out var panels))
			{
				return;
			}

			foreach(var panel in panels)
			{
				panel.SetClass( "hidden", !panel.HasClass("hidden") );
			}
		}

		public override void Tick()
		{
			if ( ListPanel == null )
				return;

			if( !createdPanels )
			{
				createdPanels = true;
				CreateCommandPanels();
			}
		}
		private void CreateCommandPanels()
		{
			foreach(var val in groupPanels.Values)
			{
				foreach(var panel in val)
				{
					panel.Delete();
				}
			}
			groupPanels.Clear();

			foreach ( var group in groups.OrderBy( group => group.Key ) )
			{
				var groupPanel = ListPanel.AddChild<Panel>( "group" );
				if ( !string.IsNullOrEmpty( group.Key ) )
				{
					groupPanel.AddClass( "valid" );
					var headerBtn = groupPanel.AddChild<Panel>( "header" );

					var commandLbl = headerBtn.AddChild<Label>( "text" );
					commandLbl.Text = group.Key;

					headerBtn.AddEventListener( "onclick", () => OnClickGroup( group.Key ) );
				}

				List<Panel> commandPanels = new();
				foreach ( var cmd in group )
				{
					var commandBtn = groupPanel.AddChild<Panel>( "command" );

					var commandLbl = commandBtn.AddChild<Label>( "text" );
					commandLbl.Text = cmd.Name;

					commandBtn.AddEventListener( "onclick", () => OnClickCommand( cmd ) );
					commandPanels.Add( commandBtn );
				}

				if ( string.IsNullOrEmpty( group.Key ) )
					continue;
				groupPanels.Add( group.Key, commandPanels );
			}
		}
		private void SetupCommandWizard(Command.ClientInfo cmd)
		{
			foreach ( var kv in parameterEntries )
			{
				kv.Value.Root.Delete(true);
			}
			parameterEntries.Clear();

			CommandTitleLabel.Text = cmd.Key.ToUpper();
			foreach(var p in cmd.Parameters)
			{ 				
				var paramPanel = ParameterPanel.AddChild<Panel>( "param" );
				var paramLbl = paramPanel.AddChild<Label>( "name" );
				paramLbl.Text = p.Name.ToUpper();

				var paramInput = paramPanel.AddChild<TextEntry>( "input" );
				paramInput.Multiline = false;
				parameterEntries.Add( p.Key, new(paramPanel, paramInput) );
			}
		}
	}
}
