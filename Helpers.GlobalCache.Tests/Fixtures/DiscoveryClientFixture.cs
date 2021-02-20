using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class DiscoveryClientFixture : IDisposable
	{
		public DiscoveryClientFixture()
		{
			var configFixture = new ConfigFixture();
			var config = configFixture.Config;
			var udpClientConfig = new Helpers.Networking.Clients.Concrete.UdpClient.Config(config.BroadcastIPAddress, config.ReceivePort);
			var udpClient = new Helpers.Networking.Clients.Concrete.UdpClient(udpClientConfig);
			DiscoveryClient = new Clients.Concrete.DiscoveryClient(udpClient);
		}

		public void Dispose() => DiscoveryClient?.Dispose();

		public Clients.IDiscoveryClient DiscoveryClient { get; }
	}
}
