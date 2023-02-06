using Sandbox;
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
		private static readonly BaseFileSystem fs = FileSystem.Mounted;
		public static Config Instance { get; private set; } = new();

		static Config()
		{
			Load();
		}

		public static void Load()
		{
			if ( !fs.FileExists( CONFIG_FILE ) )
			{
				Debug.Log( "Config file not found, creating new one." );
				Save();
				return;
			}

			var config = fs.ReadJson<Config>( CONFIG_FILE );
			if ( config == null )
			{
				Debug.Log( "Config file is invalid, creating new one." );
				Save();
				return;
			}

			Instance = config;
		}

		public static void Save()
		{
			if(Instance == null)
			{
				Instance = new();
			}

			fs.WriteJson( CONFIG_FILE, Instance );
		}

		#endregion Persistent Config

		public enum DataLocation
		{
			GlobalFile,
			PerOrgFile,
			PerGamemodeFile
		}

		public DataLocation Location { get; set; } = DataLocation.GlobalFile;
		public bool EnableMenu { get; set; } = true;
		public string DefaultUserGroup { get; set; } = "user";
		public BaseFileSystem GetFileSystem()
		{
			switch ( Location )
			{
				case DataLocation.PerOrgFile:
					return FileSystem.OrganizationData;
				case DataLocation.PerGamemodeFile:
					return FileSystem.Data;
				default:
					return FileSystem.Mounted;
			}
		}
	}
}
