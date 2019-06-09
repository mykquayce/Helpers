using Newtonsoft.Json;

namespace Helpers.Telegram.Models.Generated
{
	public class From
	{
		public int Id { get; set; }
		[JsonProperty("is_bot")]
		public bool IsBot { get; set; }
		[JsonProperty("first_name")]
		public string? FirstName { get; set; }
		public string? Username { get; set; }
	}
}
