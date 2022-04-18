using Microsoft.Extensions.Caching.Memory;

namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly IMemoryCache _memoryCache;

	public ServiceFixture()
	{
		var fixture = new SecureClientFixture();
		var client = fixture.Client;
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		Service = new Concrete.Service(client, _memoryCache);
	}

	public IService Service { get; }

	public void Dispose() => _memoryCache.Dispose();
}
