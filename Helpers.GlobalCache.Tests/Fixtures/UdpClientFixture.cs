using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class UdpClientFixture : IDisposable
	{
		public UdpClientFixture()
		{
			var config = new Clients.Concrete.UdpClient.Config();
			UdpClient = new Clients.Concrete.UdpClient(config);
		}

		public void Dispose() => UdpClient?.Dispose();

		public Clients.IUdpClient UdpClient { get; }
	}
}
