using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Helpers.Identity.Tests;

public sealed class SecureWebClientBaseTests : IClassFixture<Fixtures.ConfigurationFixture>, IDisposable
{
	private readonly WebApplicationFactory<Helpers.Identity.Tests.TestApi.Program> _webApplicationFactory;
	private readonly HttpClient _apiHttpClient, _identityHttpClient;
	private readonly IMemoryCache _memoryCache;
	private readonly Clients.ISecureWebClient _sut;

	public SecureWebClientBaseTests(Fixtures.ConfigurationFixture fixture)
	{
		_webApplicationFactory = new();
		_apiHttpClient = _webApplicationFactory.CreateClient();
		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_identityHttpClient = new HttpClient(handler) { BaseAddress = fixture.Authority, };
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		var identityClient = new Identity.Clients.Concrete.IdentityClient(fixture.Config, _identityHttpClient, _memoryCache);
		_sut = new Clients.Concrete.SecureWebClient(_apiHttpClient, identityClient);
	}

	public void Dispose()
	{
		_memoryCache.Dispose();
		_identityHttpClient.Dispose();
		_apiHttpClient.Dispose();
	}

	[Fact]
	public async Task Test1()
	{
		var json = await _sut.GetStringAsync(new Uri("weatherforecast", UriKind.Relative));
		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.StartsWith("[", json);
	}

	[Theory]
	[InlineData(5)]
	public async Task CacheTests(int count)
	{
		var times = new List<TimeSpan>();
		var stopwatch = Stopwatch.StartNew();

		while (count-- > 0)
		{
			var json = await _sut.GetStringAsync(new Uri("weatherforecast", UriKind.Relative));
			times.Add(stopwatch.Elapsed);
			stopwatch.Restart();
		}

		var first = times.First();
		var remainder = times.Skip(1).ToList();

		Assert.All(remainder, ts => Assert.True(ts < first));
	}
}
