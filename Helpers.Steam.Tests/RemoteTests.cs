using System.Net;
using System.Text.Json;
using Xunit;

namespace Helpers.Steam.Tests
{
	public class RemoteTests : IClassFixture<Fixtures.HttpClientFixture>, IClassFixture<Fixtures.UserSecretsFixture>
	{
		private readonly System.Net.Http.HttpClient _httpClient;
		private readonly string _key;
		private readonly IReadOnlyList<long> _steamIds;

		public RemoteTests(Fixtures.HttpClientFixture httpClientFixture, Fixtures.UserSecretsFixture userSecretsFixture)
		{
			ArgumentNullException.ThrowIfNull(httpClientFixture?.HttpClient);
			ArgumentOutOfRangeException.ThrowIfZero(userSecretsFixture?.SteamIds?.Count ?? 0);
			ArgumentException.ThrowIfNullOrWhiteSpace(userSecretsFixture?.SteamKey);

			_httpClient = httpClientFixture!.HttpClient;

			_key = userSecretsFixture!.SteamKey;

			_steamIds = userSecretsFixture.SteamIds;
		}

		[Fact]
		public async Task GetOwnedGames()
		{
			var uri = new Uri($"/IPlayerService/GetOwnedGames/v0001/?key={_key}&steamid={_steamIds[0]}&format=json", UriKind.Relative);

			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

			using var responseMessage = await _httpClient.SendAsync(requestMessage);

			using var responseContentStream = await responseMessage.Content!.ReadAsStreamAsync();

			var getOwnedGamesResponse = await JsonSerializer.DeserializeAsync<Concrete.SteamClient.GetOwnedGamesResponse>(responseContentStream);

			// Assert
			Assert.NotNull(getOwnedGamesResponse);
			Assert.NotNull(getOwnedGamesResponse!.Response);
			Assert.NotNull(getOwnedGamesResponse.Response!.GameCount);
			Assert.InRange(getOwnedGamesResponse.Response.GameCount!.Value, 1, int.MaxValue);
			Assert.NotNull(getOwnedGamesResponse.Response!.Games);
			Assert.NotEmpty(getOwnedGamesResponse.Response!.Games);

			var count = 0;

			foreach (var game in getOwnedGamesResponse.Response!.Games!)
			{
				count++;
				Assert.NotNull(game!.AppId);
				Assert.InRange(game.AppId!.Value, 0, int.MaxValue);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		[Theory]
		[InlineData(4540)]
		public async Task GetAppDetailsAsync_Failing(int appId)
		{
			var requestUri = $"https://store.steampowered.com/api/appdetails?appids={appId:D}";

			using var response = await _httpClient.GetAsync(requestUri);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(response.Content);

			var json = await response.Content!.ReadAsStringAsync();

			Assert.NotNull(json);
			Assert.Equal($@"{{""{appId:D}"":{{""success"":false}}}}", json);
		}
	}
}
