using System.Text.Json.Serialization;

namespace Helpers.Steam.Models
{
	public class Game
	{
		[JsonPropertyName("appid")]
		public int? AppId { get; set; }
		[JsonPropertyName("playtime_forever")]
		public int? PlaytimeForever { get; set; }
		[JsonPropertyName("playtime_2weeks")]
		public int? Playtime2Weeks { get; set; }
		[JsonPropertyName("playtime_windows_forever")]
		public int? PlaytimeWindowsForever { get; set; }
		[JsonPropertyName("playtime_mac_forever")]
		public int? PlaytimeMacForever { get; set; }
		[JsonPropertyName("playtime_linux_forever")]
		public int? PlaytimeLinuxForever { get; set; }
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		public int? Minutes => PlaytimeForever;
	}
}
