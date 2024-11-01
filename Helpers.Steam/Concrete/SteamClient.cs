﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using System.Text.Json.Serialization;

namespace Helpers.Steam.Concrete
{
	public class SteamClient : Helpers.Web.WebClientBase, ISteamClient
	{
		private readonly string _key;

		private const string _baseUri = "https://api.steampowered.com";
		private readonly static Uri _baseAddress = new(_baseUri, UriKind.Absolute);
		private readonly static HttpMessageHandler _httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		private readonly static HttpClient _httpClient = new(_httpClientHandler) { BaseAddress = _baseAddress, };

		public SteamClient(
			IOptions<Config.Settings> settingsOptions,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(_httpClient, logger, tracer)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(settingsOptions?.Value?.Key);
			_key = settingsOptions!.Value!.Key;
		}

		public SteamClient(
			string key,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(_httpClient, logger, tracer)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(key);
			_key = key;
		}

		public async Task<Models.AppDetails> GetAppDetailsAsync(int appId)
		{
			var attempts = 0;
			var uri = new Uri($"https://store.steampowered.com/api/appdetails?appids={appId:D}", UriKind.Absolute);

			while (attempts++ < 10)
			{
				var response = await base.SendAsync<AppsDetailsResponse>(HttpMethod.Get, uri);

				switch (response.StatusCode)
				{
					case System.Net.HttpStatusCode.OK:
						return response.Object?[appId].Data
							?? throw new Exceptions.AppNotFoundException(appId);
					case System.Net.HttpStatusCode.BadRequest:
					case System.Net.HttpStatusCode.InternalServerError:
					case System.Net.HttpStatusCode.TooManyRequests:
						await Task.Delay(millisecondsDelay: 60_000);
						continue;
					default:
						throw new Exception("Unexpected response from third-party API: " + response.StatusCode)
						{
							Data = { [nameof(appId)] = appId, },
						};
				}
			}

			throw new Exceptions.AppNotFoundException(appId);
		}

		public async IAsyncEnumerable<Models.Game> GetOwnedGamesAsync(long steamId)
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(steamId);

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

		public class AppsDetailsResponse : Dictionary<string, AppsDetailsResponse.AppDetailsResponse>
		{
			public AppDetailsResponse this[int i]
				=> this[i.ToString("D")];

			public class AppDetailsResponse
			{
				[JsonPropertyName("success")]
				public bool? Success { get; set; }
				[JsonPropertyName("data")]
				public Models.AppDetails? Data { get; set; }
			}
		}

	}
}
