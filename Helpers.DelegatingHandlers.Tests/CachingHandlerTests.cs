using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace Helpers.DelegatingHandlers.Tests;

public class CachingHandlerTests
{
	[Theory]
	[InlineData(10)]
	public async Task ResultsAreCachedLocallyTests(int count)
	{
		var guids = new List<Guid>(capacity: count);

		var provider = new ServiceCollection()
			.AddMemoryCache()
			.AddSingleton(RandomNumberGenerator.Create())
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddCachingHandler(c => c.Expiration = TimeSpan.FromMinutes(5))
			.AddTransient<MockingHandler>()
			.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<TestClient>();

		while (count-- > 0)
		{
			var bytes = await sut.GetByteArrayAsync(count: 16);
			var guid = new Guid(bytes);
			guids.Add(guid);
		}

		Assert.NotEmpty(guids);
		Assert.DoesNotContain(default, guids);
		Assert.Single(guids.Distinct());
	}
}
