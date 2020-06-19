namespace Helpers.Twitch.Models
{
	public class GetTokenResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? access_token { get; set; }
		public int? expires_in { get; set; }
		public string? token_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
