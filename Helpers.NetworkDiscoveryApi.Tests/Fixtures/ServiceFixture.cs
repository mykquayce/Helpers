using Microsoft.Extensions.Caching.Memory;

namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly SecureClientFixture _secureClientFixture;
	private readonly IMemoryCache _memoryCache;

	public ServiceFixture()
	{
		var configFixture = new ConfigurationFixture();
		_secureClientFixture = new SecureClientFixture();
		var client = _secureClientFixture.Client;
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		Service = new Concrete.Service(client, _memoryCache, configFixture.Aliases);
	}

	public IService Service { get; }

	public void Dispose()
	{
		_memoryCache.Dispose();
		_secureClientFixture.Dispose();
	}
}
