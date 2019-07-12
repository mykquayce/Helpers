using Helpers.Telegram.Models.Generated;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Telegram.Models.Tests
{
	public class ResultTests
	{
		[Fact]
		public void ResultTests_Deserialize()
		{
			// Arrange
			var jsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};

			var json = @"{
				""ok"": true,
				""result"": {
					""message_id"": 4,
					""from"": {
						""id"": 898512484,
						""is_bot"": true,
						""first_name"": ""CCQEnzJNJotSNA97Yfyy"",
						""username"": ""movie_times_bot""
					},
					""chat"": {
						""id"": -396035426,
						""title"": ""3JSSAxvnu1dYZbqGtaaz"",
						""type"": ""group"",
						""all_members_are_administrators"": true
					},
					""date"": 1559589762,
					""text"": ""my sample text""
				}
			}";

			// Act
			var actual = JsonSerializer.Parse<Response>(json, jsonSerializerOptions);

			// Assert
			Assert.NotNull(actual);
			Assert.True(actual.Ok);
			Assert.NotNull(actual.Result);
			Assert.Equal(4, actual.Result.MessageId);
			Assert.NotNull(actual.Result.From);
			Assert.Equal(898512484, actual.Result.From.Id);
			Assert.True(actual.Result.From.IsBot);
			Assert.Equal("CCQEnzJNJotSNA97Yfyy", actual.Result.From.FirstName);
			Assert.Equal("movie_times_bot", actual.Result.From.Username);
			Assert.NotNull(actual.Result.Chat);
			Assert.Equal(-396035426, actual.Result.Chat.Id);
			Assert.Equal("3JSSAxvnu1dYZbqGtaaz", actual.Result.Chat.Title);
			Assert.Equal("group", actual.Result.Chat.Type);
			Assert.True(actual.Result.Chat.AllMembersAreAdministrators);
			Assert.Equal(1559589762, actual.Result.Date);
			Assert.Equal("my sample text", actual.Result.Text);
		}
	}
}
