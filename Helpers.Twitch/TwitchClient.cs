using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.Twitch
{
	public class TwitchClient : IDisposable
	{
		private readonly HttpMessageHandler _httpMessageHandler;
		private readonly HttpClient _apiHttpClient, _authHttpClient;
		private readonly string _clientId, _clientSecret;
		private readonly string? _bearerToken;

		public TwitchClient(Models.Config config)
		{
			if (config is null) throw new ArgumentNullException(nameof(config));
			_bearerToken = config.BearerToken;
			_clientId = config.ClientId ?? throw new ArgumentNullException(nameof(Models.Config.ClientId));
			_clientSecret = config.ClientSecret ?? throw new ArgumentNullException(nameof(Models.Config.ClientSecret));

			_httpMessageHandler = new HttpClientHandler { AllowAutoRedirect = false, };

			_apiHttpClient = new HttpClient(_httpMessageHandler)
			{
				BaseAddress = new Uri("https://api.twitch.tv"),
				DefaultRequestHeaders =
				{
					{ "Authorization", "Bearer " + _bearerToken },
					{ "Client-ID", _clientId },
				},
			};

			_authHttpClient = new HttpClient(_httpMessageHandler)
			{
				BaseAddress = new Uri("https://id.twitch.tv"),
			};
		}

		public async Task<(string token, DateTime expiry)> GetTokenAsync()
		{
			var requestUri = $"/oauth2/token?client_id={_clientId}&client_secret={_clientSecret}&grant_type=client_credentials";

			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

			using var responseMessage = await _authHttpClient.PostAsync(requestUri, default);

			using var stream = await responseMessage.Content!.ReadAsStreamAsync();

			var o = await JsonSerializer.DeserializeAsync<Models.GetTokenResponseObject>(stream);

			var token = o!.access_token ?? throw new ArgumentNullException(nameof(o.access_token));
			var expiresIn = o!.expires_in ?? throw new ArgumentNullException(nameof(o.expires_in));
			var expiry = DateTime.UtcNow.AddSeconds(expiresIn);

			return (token, expiry);
		}

		public async IAsyncEnumerable<(int id, string login)> GetUsersAsync(params string[] logins)
		{
			var requestUri = "/helix/users?" + string.Join('&', logins.Select(l => "login=" + l));

			using var responseMessage = await _apiHttpClient.GetAsync(requestUri);

			using var stream = await responseMessage.Content!.ReadAsStreamAsync();

			var o = await JsonSerializer.DeserializeAsync<Models.GetUsersResponseObject>(stream);

			foreach (var user in o.data!)
			{
				var id = int.Parse(user.id);

				yield return (id, user.login!);
			}
		}

		public void Dispose()
		{
			_authHttpClient?.Dispose();
			_httpMessageHandler?.Dispose();
		}
	}
}
