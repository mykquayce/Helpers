using Helpers.PhilipsHue.Clients;
using Helpers.PhilipsHue.Clients.Concrete;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace Helpers.PhilipsHue.Tests.Fixtures
{
	public sealed class ClientFixture : IDisposable
	{
		private const string _section = "PhilipsHue";

		public ClientFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();
			var config = userSecretsFixture.GetSection<PhilipsHueClient.Config>(_section);
			var physicalAddress = PhysicalAddress.Parse(config.BridgePhysicalAddress);
			var ipAddress = Helpers.Networking.NetworkHelpers.IPAddressFromPhysicalAddress(physicalAddress);
			var baseAddress = new Uri("http://" + ipAddress.ToString());
			var httpClient = new HttpClient { BaseAddress = baseAddress, };
			Client = new PhilipsHueClient(httpClient, config);
		}

		public void Dispose() => Client.Dispose();

		public IPhilipsHueClient Client { get; }
	}
}
