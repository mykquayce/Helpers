using System;
using System.Net.Http;

namespace Helpers.Steam.Tests.Fixtures
{
	public class HttpClientFixture : IDisposable
	{
		private readonly HttpMessageHandler _httpClientHandler;

		public HttpClientFixture()
		{
			_httpClientHandler = new HttpClientHandler
			{
				AllowAutoRedirect = false,
			};

			HttpClient = new System.Net.Http.HttpClient(_httpClientHandler)
			{
				BaseAddress = new Uri("https://api.steampowered.com", UriKind.Absolute),
			};
		}

		public System.Net.Http.HttpClient HttpClient { get; }

		public void Dispose()
		{
			_httpClientHandler?.Dispose();
			HttpClient?.Dispose();
		}
	}
}
