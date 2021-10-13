using Helpers.Steam.Concrete;
using Helpers.Steam.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
			var game = JsonSerializer.Deserialize<Game>(json);

			// Assert
			Assert.NotNull(game);
			Assert.Equal(249130, game!.AppId);
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

		[Theory]
		[InlineData(@"{""249130"":""test""}", 249130)]
		public void DeserializeUnknownKeyTests(string json, int expected)
		{
			var o = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

			Assert.NotNull(o);
			Assert.NotEmpty(o);

			var ok = int.TryParse(o!.Keys.First(), out var actual);

			Assert.True(ok);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("appdetails_249130.json", "249130", "game")]
		public async Task DeserializeCategoriesTest(string fileName, string appId, string expectedType)
		{
			// Act
			var response = await HelperMethods.ReadAndDeserializeFileAsync<Dictionary<string, AppDetailsResponse>>(fileName);

			// Assert
			Assert.NotNull(response);
			Assert.NotEmpty(response);
			Assert.True(response.ContainsKey(appId));
			Assert.NotNull(response[appId]);
			Assert.True(response[appId].Success);
			Assert.NotNull(response[appId].Data);
			Assert.Equal(expectedType, response[appId].Data!.Type);
			Assert.NotNull(response[appId].Data!.Categories);
			Assert.NotEmpty(response[appId].Data!.Categories);
		}


		public class AppDetailsResponse
		{
			[JsonPropertyName("success")]
			public bool? Success { get; set; }
			[JsonPropertyName("data")]
			public Models.AppDetails? Data { get; set; }
		}

		[Fact]
		public void GetAllCategoryIds()
		{

		}

		[Theory]
		[InlineData(
			"appdetails_249130.json",
			Category.SinglePlayer, Category.Multiplayer, Category.SharedSplitScreen, Category.SteamAchievements,
			Category.FullControllerSupport, Category.SteamCloud, Category.RemotePlayOnPhone,
			Category.RemotePlayOnTablet, Category.RemotePlayOnTV, Category.RemotePlayTogether)]
		public async Task CategoryIdsTest(string fileName, params Category[] expected)
		{
			// Arrange
			var response = await HelperMethods.ReadAndDeserializeFileAsync<Dictionary<string, AppDetailsResponse>>(fileName);

			// Act
			var actual = response!.TryGetValue("249130", out var app)
				? app.Data?.Categories.ToList()
				: default;

			// Assert
			Assert.Equal(expected, actual!);
		}

		[Theory]
		[InlineData("appdetails_221680.json")]
		public Task AppDetailsTest(string fileName)
		{
			return HelperMethods.ReadAndDeserializeFileAsync<SteamClient.AppsDetailsResponse>(fileName);
		}

		[Theory]
		[InlineData("GetOwnedGames_76561197965007048.json")]
		public async Task GetOwnedGames(string fileName)
		{
			// Act
			var response = await HelperMethods.ReadAndDeserializeFileAsync<Concrete.SteamClient.GetOwnedGamesResponse>(fileName);

			// Assert
			Assert.NotNull(response);
			Assert.NotNull(response.Response);
			Assert.NotNull(response.Response!.Games);
			Assert.NotEmpty(response.Response.Games);

			foreach (var game in response.Response!.Games!)
			{
				Assert.InRange(game.AppId ?? 0, 1, int.MaxValue);
				Assert.InRange(game.Minutes ?? 0, 0, int.MaxValue);
			}
		}
	}
}
