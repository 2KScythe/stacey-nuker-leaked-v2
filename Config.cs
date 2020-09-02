using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Quazide
{
	public class Config
	{

		[JsonProperty("token")]
		public string Token { get; private set; }

		[JsonProperty("create_channels_amount")]
		public int CreateChannelsAmount { get; private set; }

		[JsonProperty("create_channels_name")]
		public string CreateChannelsName { get; private set; }
    
		[JsonProperty("cmd_mode")]
		public bool UseCommand { get; private set; }

		[JsonProperty("prefix")]
		public string Prefix { get; private set; }
    
		[JsonProperty("command")]
		public string Command { get; private set; }

		[JsonProperty("cmd_whitelist")]
		public List<ulong> CmdWhitelist { get; private set; }

		public static Config Load()
		{
			return JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));
		}
	}
}
