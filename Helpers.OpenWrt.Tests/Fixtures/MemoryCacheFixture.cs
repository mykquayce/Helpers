using Microsoft.Extensions.Caching.Memory;

namespace Helpers.OpenWrt.Tests.Fixtures;

public sealed class MemoryCacheFixture : IDisposable
{
	public IMemoryCache MemoryCache { get; } = new MemoryCache(new MemoryCacheOptions());

	public void Dispose() => MemoryCache.Dispose();
}
