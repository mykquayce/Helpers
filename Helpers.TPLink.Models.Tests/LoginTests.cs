using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Models.Tests
{
	public class LoginTests
	{
		[Theory]
		[InlineData(
			"v.reed@randatmail.com",
			"90f172aa245493dc02f45e9d88916980311f69c1c55569e94b07239d987cf6b2",
			"f8d2f7c6-ab5f-423d-8d3f-64a324455a4b",
			@"{
  ""method"": ""login"",
  ""params"": {
    ""appType"": ""Kasa_Android"",
    ""cloudUserName"": ""v.reed@randatmail.com"",
    ""cloudPassword"": ""90f172aa245493dc02f45e9d88916980311f69c1c55569e94b07239d987cf6b2"",
    ""terminalUUID"": ""f8d2f7c6-ab5f-423d-8d3f-64a324455a4b""
  }
}")]
		public void BuildLoginRequestObject_IsAsExpecetd(string cloudUserName, string cloudPassword, string terminalUUID, string expected)
		{
			var before = new LoginRequestObject
			{
				@params = new LoginRequestObject.LoginParamsOjbect
				{
					cloudUserName = cloudUserName,
					cloudPassword = cloudPassword,
					terminalUUID = terminalUUID,
				},
			};

			var options = new JsonSerializerOptions { WriteIndented = true, };
			var actual = JsonSerializer.Serialize(before, options);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(@"{
    ""error_code"": 0,
    ""result"": {
        ""accountId"": ""40239856"",
        ""regTime"": ""2020-09-27 18:21:38"",
        ""email"": ""o.robinson@randatmail.com"",
        ""token"": ""6bdd39aa-ocsdaZ9u8L7wZOaeG2q4cI6""
    }
}",
			Enums.ErrorCode.None, "40239856", "2020-09-27 18:21:38", "o.robinson@randatmail.com", "6bdd39aa-ocsdaZ9u8L7wZOaeG2q4cI6")]
		public void DeserializeLoginResponseJson_HasTheCorrectValues(
			string json,
			Enums.ErrorCode error_code, string accountId, string regTimeString, string email, string token)
		{
			var actual = JsonSerializer.Deserialize<LoginResponseObject>(json);

			Assert.NotNull(actual);
			Assert.Equal(error_code, actual!.error_code);
			Assert.NotNull(actual.result);
			Assert.Equal(accountId, actual.result!.accountId);
			Assert.Equal(regTimeString, actual.result.regTime);
			Assert.Equal(email, actual.result.email);
			Assert.Equal(token, actual.result.token);
		}
	}
}
