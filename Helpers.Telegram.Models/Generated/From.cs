using System.Text.Json.Serialization;

namespace Helpers.Telegram.Models.Generated
{
	public class From
	{
		public int Id { get; set; }
		[JsonPropertyName("is_bot")]
		public bool IsBot { get; set; }
		[JsonPropertyName("first_name")]
		public string? FirstName { get; set; }
		public string? Username { get; set; }
	}
}
