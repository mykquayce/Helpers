using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class GlobalCacheServiceFixture : IDisposable
	{
		private readonly SocketClientFixture _socketClientFixture;
		private readonly UdpClientFixture _udpClientFixture;

		public GlobalCacheServiceFixture()
		{
			_socketClientFixture = new SocketClientFixture();
			_udpClientFixture = new UdpClientFixture();

			var config = new Services.Concrete.GlobalCacheService.Config();

			GlobalCacheService = new Services.Concrete.GlobalCacheService(
				config,
				_socketClientFixture.SocketClient,
				_udpClientFixture.UdpClient);
		}

		public void Dispose()
		{
			GlobalCacheService?.Dispose();
			_udpClientFixture?.Dispose();
			_socketClientFixture?.Dispose();
		}

		public Services.IGlobalCacheService GlobalCacheService { get; }
	}
}
