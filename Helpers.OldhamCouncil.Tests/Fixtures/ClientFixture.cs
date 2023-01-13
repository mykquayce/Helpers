using System;
using System.Net.Http;

namespace Helpers.OldhamCouncil.Tests.Fixtures
{
	public sealed class ClientFixture : IDisposable
	{
		private readonly Uri _baseAddress = new("https://portal.oldham.gov.uk/", UriKind.Absolute);
		private readonly HttpClient _httpClient;

		public ClientFixture()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			_httpClient = new HttpClient(handler) { BaseAddress = _baseAddress, };
			Client = new Concrete.Client(_httpClient);
		}

		public IClient Client { get; }

		public void Dispose() => _httpClient?.Dispose();
	}
}
