using Helpers.PhilipsHue.Clients;
using Helpers.PhilipsHue.Clients.Concrete;
using System;
using System.Net.Http;

namespace Helpers.PhilipsHue.Tests.Fixtures
{
	public sealed class ClientFixture : IDisposable
	{
		private const string _section = "PhilipsHue";

		public ClientFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();
			var config = userSecretsFixture.GetSection<PhilipsHueClient.Config>(_section);
			var bridgeHostName = userSecretsFixture["PhilipsHue:BridgeHostName"];
			var baseAddress = new Uri("http://" + bridgeHostName);
			var httpClient = new HttpClient { BaseAddress = baseAddress, };
			Client = new PhilipsHueClient(httpClient, config);
		}

		public void Dispose() => Client.Dispose();

		public IPhilipsHueClient Client { get; }
	}
}
