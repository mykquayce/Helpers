using System.Text.Json;
using Xunit;

namespace Helpers.Steam.Tests
{
	public class SerializationTests
	{
		[Theory]
		[InlineData(@"{
				""appid"": 249130,
				""name"": ""LEGO® MARVEL Super Heroes"",
				""playtime_2weeks"": 34,
				""playtime_forever"": 189,
				""img_icon_url"": ""8462ed9e1004624c7109233eafd7098211da9f86"",
				""img_logo_url"": ""8b012ffdf2cb13e6f71cdf3606ce053c888ff3b5"",
				""has_community_visible_stats"": true,
				""playtime_windows_forever"": 34,
				""playtime_mac_forever"": 0,
				""playtime_linux_forever"": 0
			}")]
		public void Game(string json)
		{
			// Act
			var game = JsonSerializer.Deserialize<Models.Game>(json);

			// Assert
			Assert.NotNull(game);
			Assert.Equal(249130, game.AppId);
			Assert.Equal("LEGO® MARVEL Super Heroes", game.Name);
		}

		[Theory]
		[InlineData(@"{
				""response"": {
					""game_count"": 270,
					""games"": [
						{
							""appid"": 240,
							""playtime_forever"": 499,
							""playtime_windows_forever"": 0,
							""playtime_mac_forever"": 0,
							""playtime_linux_forever"": 0
						},
						{
							""appid"": 3830,
							""playtime_forever"": 758,
							""playtime_windows_forever"": 0,
							""playtime_mac_forever"": 0,
							""playtime_linux_forever"": 0
						}
					]
				}
			}")]
		public void GetOwnedGamesResponse(string json)
		{
			// Act
			var getOwnedGamesResponse = JsonSerializer.Deserialize<Concrete.SteamClient.GetOwnedGamesResponse>(json);

			// Assert
			Assert.NotNull(getOwnedGamesResponse);
			Assert.NotNull(getOwnedGamesResponse!.Response);
			Assert.NotNull(getOwnedGamesResponse.Response!.GameCount);
			Assert.InRange(getOwnedGamesResponse.Response.GameCount!.Value, 1, int.MaxValue);
			Assert.NotNull(getOwnedGamesResponse.Response!.Games);
			Assert.NotEmpty(getOwnedGamesResponse.Response!.Games);

			foreach (var game in getOwnedGamesResponse.Response!.Games!)
			{
				Assert.NotNull(game!.AppId);
				Assert.InRange(game.AppId!.Value, 0, int.MaxValue);
				Assert.InRange(game.Minutes!.Value, 0, int.MaxValue);
			}
		}
	}
}
