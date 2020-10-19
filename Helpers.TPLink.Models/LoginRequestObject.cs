namespace Helpers.TPLink.Models
{
	public class LoginRequestObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? method { get; } = "login";
		public LoginParamsOjbect? @params { get; init; }

		public class LoginParamsOjbect
		{
			public string? appType { get; } = "Kasa_Android";
			public string? cloudUserName { get; init; }
			public string? cloudPassword { get; init; }
			public string? terminalUUID { get; init; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
