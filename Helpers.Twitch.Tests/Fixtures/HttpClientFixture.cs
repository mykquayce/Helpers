using System;
using System.Net.Http;

namespace Helpers.Twitch.Tests.Fixtures
{
	public class HttpClientFixture : IDisposable
	{
		private readonly HttpMessageHandler _httpMessageHandler;

		public HttpClientFixture()
		{
			_httpMessageHandler = new HttpClientHandler { AllowAutoRedirect = false, };
			HttpClient = new HttpClient(_httpMessageHandler);
		}

		public HttpClient HttpClient { get; }

		public void Dispose()
		{
			HttpClient?.Dispose();
			_httpMessageHandler?.Dispose();
		}
	}
}
