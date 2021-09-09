using Microsoft.Extensions.Caching.Memory;
using System.Xml.Serialization;

namespace Helpers.Cineworld.Tests.Fixtures;

public sealed class ClientFixture : IDisposable
{
	private readonly HttpClientHandler _httpClientHandler;
	private readonly HttpClient _httpClient;
	private readonly IMemoryCache _memoryCache;

	public ClientFixture()
	{
		var config = Concrete.Client.Config.Defaults with { CacheExpiration = TimeSpan.FromSeconds(10), };
		_httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(_httpClientHandler) { BaseAddress = config.BaseAddressUri, };
		var xmlSerializerFactory = new XmlSerializerFactory();
		_memoryCache = new MemoryCache(new MemoryCacheOptions());

		Client = new Concrete.Client(config, _httpClient, xmlSerializerFactory, _memoryCache);
	}

	public IClient Client { get; }

	public void Dispose()
	{
		_memoryCache.Dispose();
		_httpClient.Dispose();
		_httpClientHandler.Dispose();
	}
}
