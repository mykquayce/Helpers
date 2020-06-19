namespace Helpers.Twitch.Models
{
	public class GetUsersResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public UserObject[]? data { get; set; }

		public class UserObject
		{
			public string? id { get; set; }
			public string? login { get; set; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
