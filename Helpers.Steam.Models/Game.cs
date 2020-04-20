using System.Text.Json.Serialization;

namespace Helpers.Steam.Models
{
	public class Game
	{
		[JsonPropertyName("appid")]
		public int? AppId { get; set; }
		[JsonPropertyName("playtime_forever")]
		public int? Minutes { get; set; }
		[JsonPropertyName("name")]
		public string? Name { get; set; }
	}
}
