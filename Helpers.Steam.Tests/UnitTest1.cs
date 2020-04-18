using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Steam.Tests
{
	public class UnitTest1
	{
		[Theory]
		[InlineData("E53F5DB4E02115972AF358A3B2D99086", "76561198025438682")]
		public async Task Test1(string key, string steamId)
		{
			var uri = new Uri($"/IPlayerService/GetOwnedGames/v0001/?key={key}&steamid={steamId}&format=json", UriKind.Relative);

			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

			using var handler = new HttpClientHandler
			{
				AllowAutoRedirect = false,
			};

			using var client = new System.Net.Http.HttpClient(handler)
			{
				BaseAddress = new Uri("http://api.steampowered.com", UriKind.Absolute),
			};

			using var responseMessage = await client.SendAsync(requestMessage);

			using var responseContentStream = await responseMessage.Content.ReadAsStreamAsync();

			var getOwnedGamesResponse = await JsonSerializer.DeserializeAsync<SteamClient.GetOwnedGamesResponse>(responseContentStream);

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
				Assert.InRange(game.PlaytimeForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeWindowsForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.FlaytimeMacForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeLinuxForever!.Value, 0, int.MaxValue);
			}
		}

		[Theory]
		[InlineData(@"{
				""appid"": 400,
				""playtime_2weeks"": 208,
				""playtime_forever"": 766,
				""playtime_windows_forever"": 208,
				""playtime_mac_forever"": 0,
				""playtime_linux_forever"": 0
			}")]
		public void Test2(string json)
		{
			// Act
			var game = JsonSerializer.Deserialize<Models.Game>(json);

			// Assert
			Assert.NotNull(game);
			Assert.NotNull(game!.AppId);
			Assert.InRange(game.AppId!.Value, 0, int.MaxValue);
			Assert.InRange(game.Playtime2Weeks!.Value, 0, int.MaxValue);
			Assert.InRange(game.PlaytimeForever!.Value, 0, int.MaxValue);
			Assert.InRange(game.PlaytimeWindowsForever!.Value, 0, int.MaxValue);
			Assert.InRange(game.FlaytimeMacForever!.Value, 0, int.MaxValue);
			Assert.InRange(game.PlaytimeLinuxForever!.Value, 0, int.MaxValue);
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
		public void Test3(string json)
		{
			// Act
			var getOwnedGamesResponse = JsonSerializer.Deserialize<SteamClient.GetOwnedGamesResponse>(json);

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
				Assert.InRange(game.PlaytimeForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeWindowsForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.FlaytimeMacForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeLinuxForever!.Value, 0, int.MaxValue);
			}
		}

		[Theory]
		[InlineData("E53F5DB4E02115972AF358A3B2D99086", "76561198025438682")]
		public async Task GetOwnedGamesTest(string key, string steamId)
		{
			var settings = new Models.Settings { Key = key, };

			var optionsSettings = Microsoft.Extensions.Options.Options.Create<Models.Settings>(settings);

			using var steamClient = new SteamClient(optionsSettings);

			var count = 0;

			await foreach(var game in steamClient.GetOwnedGamesAsync(steamId))
			{
				count++;
				Assert.NotNull(game!.AppId);
				Assert.InRange(game.AppId!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeWindowsForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.FlaytimeMacForever!.Value, 0, int.MaxValue);
				Assert.InRange(game.PlaytimeLinuxForever!.Value, 0, int.MaxValue);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}
	}
}
