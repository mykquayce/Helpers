using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class SocketClientFixture : IDisposable
	{
		public SocketClientFixture()
		{
			var config = new Helpers.Networking.Clients.Concrete.SocketClient.Config();
			SocketClient = new Helpers.Networking.Clients.Concrete.SocketClient(config);
		}

		public void Dispose() => SocketClient?.Dispose();

		public Helpers.Networking.Clients.ISocketClient SocketClient { get; }
	}
}
