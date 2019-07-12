using System.Text.Json.Serialization;

namespace Helpers.Telegram.Models.Generated
{
	public class Result
	{
		[JsonPropertyName("message_id")]
		public int MessageId { get; set; }
		public From? From { get; set; }
		public Chat? Chat { get; set; }
		public int Date { get; set; }
		public string? Text { get; set; }
	}
}
