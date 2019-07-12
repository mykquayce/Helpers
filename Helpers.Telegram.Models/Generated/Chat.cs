using System.Text.Json.Serialization;

namespace Helpers.Telegram.Models.Generated
{
	public class Chat
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Type { get; set; }
		[JsonPropertyName("all_members_are_administrators")]
		public bool AllMembersAreAdministrators { get; set; }
	}
}
