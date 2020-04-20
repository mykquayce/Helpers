using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Helpers.Steam.Concrete
{
	public class SteamClient : Helpers.HttpClient.HttpClientBase, IDisposable, ISteamClient
	{
		private readonly string _key;

		private const string _baseUri = "https://api.steampowered.com";
		private readonly static Uri _baseAddress = new Uri(_baseUri, UriKind.Absolute);
		private readonly static HttpMessageHandler _httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		private readonly static System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient(_httpClientHandler) { BaseAddress = _baseAddress, };

		public SteamClient(
			IOptions<Models.Settings> settingsOptions,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(_httpClient, logger, tracer)
		{
			_key = Guard.Argument(() => settingsOptions).NotNull()
				.Wrap(o => o.Value).NotNull()
				.Wrap(v => v.Key!).NotNull().NotEmpty().NotWhiteSpace().Value;
		}

		public SteamClient(
			string key,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(_httpClient, logger, tracer)
		{
			_key = Guard.Argument(() => key).NotNull().NotEmpty().NotWhiteSpace().Value;
		}

		public async IAsyncEnumerable<Models.Game> GetOwnedGamesAsync(long steamId)
		{
			Guard.Argument(() => steamId).Positive();

			var uri = new Uri($"/IPlayerService/GetOwnedGames/v0001/?key={_key}&steamid={steamId:D}&format=json&include_appinfo=1&include_played_free_games=1", UriKind.Relative);

			var response = await base.SendAsync<GetOwnedGamesResponse>(HttpMethod.Get, uri);

			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception("Unexpected response from third-party API: " + response.StatusCode)
				{
					Data = { [nameof(steamId)] = steamId, },
				};
			}

			if (response.Object?.Response?.Games is null)
			{
				yield break;
			}

			foreach (var game in response.Object.Response.Games)
			{
				yield return game;
			}
		}

		public class GetOwnedGamesResponse
		{
			[JsonPropertyName("response")]
			public ResponseObject? Response { get; set; }

			public class ResponseObject
			{
				[JsonPropertyName("game_count")]
				public int? GameCount { get; set; }
				[JsonPropertyName("games")]
				public Models.Game[]? Games { get; set; }
			}
		}

	}
}
