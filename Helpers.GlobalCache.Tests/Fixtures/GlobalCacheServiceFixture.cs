using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class GlobalCacheServiceFixture : IDisposable
	{
		public GlobalCacheServiceFixture()
		{
			var configFixture = new Fixtures.ConfigFixture();
			var config = configFixture.Config;

			GlobalCacheService = new Services.Concrete.GlobalCacheService(config);
		}

		public void Dispose() => GlobalCacheService.Dispose();

		public Services.IGlobalCacheService GlobalCacheService { get; }
	}
}
