using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Twitch.Tests
{
	public class UnitTest1 : IClassFixture<Fixtures.HttpClientFixture>, IClassFixture<Fixtures.UserSecretsFixture>
	{
		private readonly string _bearerToken;
		private readonly string _clientId;
		private readonly string _clientSecret;
		private readonly HttpClient _httpClient;

		public UnitTest1(
			Fixtures.HttpClientFixture httpClientFixture,
			Fixtures.UserSecretsFixture userSecretsFixture)
		{
			_httpClient = httpClientFixture.HttpClient;
			_bearerToken = userSecretsFixture.BearerToken;
			_clientId = userSecretsFixture.ClientId;
			_clientSecret = userSecretsFixture.ClientSecret;
		}

		[Fact]
		public async Task GetAppAccessTokenTests()
		{
			var queryString = $"https://id.twitch.tv/oauth2/token?client_id={_clientId}&client_secret={_clientSecret}&grant_type=client_credentials";

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, queryString);

			var responseMessage = await _httpClient.SendAsync(requestMessage);

			using var stream = await responseMessage.Content!.ReadAsStreamAsync();

			var response = await JsonSerializer.DeserializeAsync<Models.GetTokenResponseObject>(stream);

			Assert.Matches("^[0-9a-z]{30}$", response.access_token);
			Assert.NotNull(response.expires_in);
			Assert.InRange(response.expires_in!.Value, 1, int.MaxValue);
			Assert.Equal("bearer", response.token_type);
		}

		[Theory]
		[InlineData("mykquayce")]
		public async Task GetUsersTests(params string[] logins)
		{
			var queryString = "https://api.twitch.tv/helix/users?" + string.Join('&', logins.Select(l => "login=" + l));

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, queryString);

			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
			requestMessage.Headers.Add("Client-ID", _clientId);

			//_httpClient.DefaultRequestHeaders.Clear();
			//_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v5+json"));
			//_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "t7baxtmpnpqjw2xcp0rlsuxga1l5q5");
			//_httpClient.DefaultRequestHeaders.Add("Client-ID", _clientId);

			//requestMessage.Headers.Accept.Add( = "application/vnd.twitchtv.v5+json";

			var responseMessage = await _httpClient.SendAsync(requestMessage);

			using var stream = await responseMessage.Content!.ReadAsStreamAsync();

			var response = await JsonSerializer.DeserializeAsync<Models.GetUsersResponseObject>(stream);

			var count = 0;

			Assert.NotNull(response);
			Assert.NotNull(response.data);

			foreach (var user in response.data!)
			{
				count++;
				Assert.Matches("^[0-9]+$", user.id);
				Assert.Matches("^[_0-9a-z]+$", user.login);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		[Theory]
		[InlineData(59447699)]
		public async Task GetFollowingTests(int id)
		{
			var queryString = $"https://api.twitch.tv/helix/users/follows?first=100&from_id={id:D}";
		}
	}
}
