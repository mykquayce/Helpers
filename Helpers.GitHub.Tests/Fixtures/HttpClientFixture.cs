using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Helpers.GitHub.Tests.Fixtures
{
	public sealed class HttpClientFixture : IDisposable
	{
		public HttpClientFixture()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			HttpClient = new HttpClient(handler)
			{
				BaseAddress = new Uri("https://api.github.com"),
				DefaultRequestHeaders =
				{
					Accept =
					{
						new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"),
					},
					UserAgent =
					{
						new ProductInfoHeaderValue("PostmanRuntime", "7.26.8"),
					},
				}
			};
		}

		public void Dispose() => HttpClient?.Dispose();

		public HttpClient HttpClient { get; }
	}
}
