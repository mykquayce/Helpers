using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Helpers.Elgato.Tests.Fixtures
{
	public sealed class HttpClientFixture : IDisposable
	{
		public HttpClientFixture()
		{
			var config = new Helpers.XUnitClassFixtures.UserSecretsFixture().Configuration;
			var endPointString = config["Elgato:EndPoint"] ?? throw new KeyNotFoundException("endpoint not found in config");
			var endPoint = IPEndPoint.Parse(endPointString);
			var baseAddress = new Uri("http://" + endPoint);
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
