using System;
using System.Net.Http;

namespace Helpers.XUnitClassFixtures
{
	public class HttpClientFixture : IDisposable
	{
		public HttpClientFixture()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			HttpClient = new HttpClient(handler);
		}

		public HttpClient HttpClient { get; }

		public void Dispose()
		{
			HttpClient?.Dispose();
		}
	}
}
