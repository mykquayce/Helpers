using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Helpers.Identity.Tests;

public sealed class SecureWebClientBaseTests : IDisposable
{
	private readonly HttpClient _apiHttpClient, _identityHttpClient;
	private readonly IMemoryCache _memoryCache;
	private readonly Clients.ISecureWebClient _sut;

	public SecureWebClientBaseTests()
	{
		var config = new Config(new Uri("https://identityserver"), "client", "secret", "api1");
		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_apiHttpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api:6001"), };
		_identityHttpClient = new HttpClient(handler) { BaseAddress = config.Authority, };
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		var identityClient = new Identity.Clients.Concrete.IdentityClient(config, _identityHttpClient, _memoryCache);
		_sut = new Clients.Concrete.SecureWebClient(_apiHttpClient, identityClient);
	}

	public void Dispose()
	{
		_memoryCache.Dispose();
		_identityHttpClient.Dispose();
		_apiHttpClient.Dispose();
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Fact(Skip = "needs third party")]
	public async Task Test1()
	{
		var json = await _sut.GetStringAsync(new Uri("/identity", UriKind.Relative));
		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.StartsWith("[", json);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData(5)]
	public async Task CacheTests(int count)
	{
		var times = new List<TimeSpan>();
		var stopwatch = Stopwatch.StartNew();

		while (count-- > 0)
		{
			var json = await _sut.GetStringAsync(new Uri("/identity", UriKind.Relative));
			times.Add(stopwatch.Elapsed);
			stopwatch.Restart();
		}

		var first = times.First();
		var remainder = times.Skip(1).ToList();

		Assert.All(remainder, ts => Assert.True(ts < first));
	}
}
