using System;
using System.Collections.Generic;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class GlobalCacheServiceFixture : IDisposable
	{
		private readonly Stack<IDisposable> _disposables = new();

		public GlobalCacheServiceFixture()
		{
			var socketClientFixture = new SocketClientFixture();
			var udpClientFixture = new DiscoveryClientFixture();

			var config = new Services.Concrete.GlobalCacheService.Config();

			GlobalCacheService = new Services.Concrete.GlobalCacheService(
				config,
				socketClientFixture.SocketClient,
				udpClientFixture.DiscoveryClient);

			_disposables.Push(socketClientFixture);
			_disposables.Push(udpClientFixture);
			_disposables.Push(GlobalCacheService);
		}

		public void Dispose()
		{
			while (_disposables.TryPop(out var disposable))
			{
				disposable?.Dispose();
			}
		}

		public Services.IGlobalCacheService GlobalCacheService { get; }
	}
}
