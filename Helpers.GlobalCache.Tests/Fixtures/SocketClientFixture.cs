using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class SocketClientFixture : IDisposable
	{
		public SocketClientFixture()
		{
			SocketClient = new Clients.Concrete.SocketClient();
		}

		public void Dispose() => SocketClient?.Dispose();

		public Clients.ISocketClient SocketClient { get; }
	}
}
