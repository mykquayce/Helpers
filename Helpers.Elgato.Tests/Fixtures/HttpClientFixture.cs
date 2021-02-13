using System;
using System.Net.Http;
using Xunit;

namespace Helpers.Elgato.Tests.Fixtures
{
	public sealed class HttpClientFixture : IDisposable
	{
		public HttpClientFixture()
		{
			var userSecrets = new UserSecretsFixture();

			var ipAddress = Helpers.Networking.NetworkHelpers.IPAddressFromPhysicalAddress(userSecrets.PhysicalAddress);
			var port = userSecrets.Port;
			var baseAddress = new Uri($"http://{ipAddress}:{port:D}");
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			HttpClient = new HttpClient(handler) { BaseAddress = baseAddress, };
		}

		public void Dispose() => HttpClient?.Dispose();

		public HttpClient HttpClient { get; }
	}

	[CollectionDefinition("httpclient collection")]
	public class DatabaseCollection : ICollectionFixture<HttpClientFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
