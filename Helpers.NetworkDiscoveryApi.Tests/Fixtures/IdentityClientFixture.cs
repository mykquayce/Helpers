using Microsoft.Extensions.Caching.Memory;

namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public class IdentityClientFixture : ConfigurationFixture, IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly IMemoryCache _memoryCache;

	public IdentityClientFixture()
	{
		var config = new Identity.Config(base.Authority, base.ClientId, base.ClientSecret, base.Scope);
		var clientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(clientHandler) { BaseAddress = base.Authority, };
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		IdentityClient = new Identity.Clients.Concrete.IdentityClient(config, _httpClient, _memoryCache);
	}

	public Identity.Clients.IIdentityClient IdentityClient { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "<Pending>")]
	public void Dispose()
	{
		_memoryCache.Dispose();
		_httpClient.Dispose();
	}
}
