using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Helpers.Steam
{
	public class SteamClient : Helpers.HttpClient.HttpClientBase, IDisposable
	{
		private readonly string _key;

		private static readonly Uri _baseAddress = new Uri("http://api.steampowered.com", UriKind.Absolute);
		private static readonly HttpMessageHandler _httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		private static System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient(_httpClientHandler) { BaseAddress = _baseAddress, };

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

		public async IAsyncEnumerable<Models.Game> GetOwnedGamesAsync(string steamId)
		{
			var uri = new Uri($"/IPlayerService/GetOwnedGames/v0001/?key={_key}&steamid={steamId}&format=json", UriKind.Relative);

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
			public Response? Response { get; set; }
		}


		public class Response
		{
			[JsonPropertyName("game_count")]
			public int? GameCount { get; set; }
			[JsonPropertyName("games")]
			public Models.Game[]? Games { get; set; }
		}
	}
}
