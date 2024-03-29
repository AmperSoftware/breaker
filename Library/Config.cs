﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaker
{
	public class Config
	{
		#region Persistent Config
		const string CONFIG_FILE = "breaker.config";
		private static BaseFileSystem configfs => FileSystem.OrganizationData;
		private static BaseFileSystem fs;
		public static Config Instance { get; private set; } = new();

		[GameEvent.Entity.PostSpawn]
		[Event.Hotload]
		public static void Load()
		{
			if ( !Game.InGame ) return;
			if ( configfs == null ) return;

			if ( !configfs.FileExists( CONFIG_FILE ) )
			{
				Debug.Log( "Config file not found, creating new one." );
				Save();
			}

			var config = configfs.ReadJson<Config>( CONFIG_FILE );
			if ( config == null )
			{
				Debug.Log( "Config file is invalid, creating new one." );
				Save();
			}

			Instance = config;
			
			if ( !configfs.DirectoryExists( "breaker" ) )
				configfs.CreateDirectory( "breaker" );
			fs = configfs.CreateSubSystem( "breaker" );

			Event.Run( BRKEvent.ConfigLoaded );
		}

		public static void Save()
		{
			if(Instance == null)
			{
				Instance = new();
			}

			Debug.Log( $"Saving config to {configfs.GetFullPath(CONFIG_FILE)}" );
			configfs.WriteJson( CONFIG_FILE, Instance );
		}

		#endregion Persistent Config

		public enum DataLocation
		{
			PerOrgFile,
			PerGamemodeFile
		}

		public DataLocation Location { get; set; } = DataLocation.PerOrgFile;
		public bool EnableMenu { get; set; } = true;
		public string DefaultUserGroup { get; set; } = "user";
		public BaseFileSystem GetFileSystem()
		{
			return fs;
		}
	}
}
