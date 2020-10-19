namespace Helpers.TPLink.Models
{
	public class LoginResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public Models.Enums.ErrorCode? error_code { get; init; }
		public LoginResultObject? result { get; init; }

		public class LoginResultObject
		{
			public string? accountId { get; init; }
			public string? regTime { get; init; }
			public string? email { get; init; }
			public string? token { get; init; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
