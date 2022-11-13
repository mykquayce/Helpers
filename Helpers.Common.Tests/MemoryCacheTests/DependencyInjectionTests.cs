using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.Common.Tests.MemoryCacheTests;

public class DependencyInjectionTests
{
	[Theory]
	[InlineData("key", "value")]
	public void AddToEnumerableMemoryCache_ConsumeFromMemoryCache(string key, string value)
	{
		using var provider = new ServiceCollection()
			.AddEnumerableMemoryCache(new MemoryCacheOptions())
			.BuildServiceProvider();

		var first = provider.GetRequiredService<IEnumerableMemoryCache>();
		var second = provider.GetRequiredService<IMemoryCache>();

		first.Set(key, value);

		Assert.Equal(value, second.Get(key));
	}

	[Theory]
	[InlineData("key", "value")]
	public void AddToEnumerableMemoryCache_NonNullLoggerFactory_ConsumeFromMemoryCache(string key, string value)
	{
		using var provider = new ServiceCollection()
			.AddLogging()
			.AddEnumerableMemoryCache(new MemoryCacheOptions())
			.BuildServiceProvider();

		var first = provider.GetRequiredService<IEnumerableMemoryCache>();
		var second = provider.GetRequiredService<IMemoryCache>();

		first.Set(key, value);

		Assert.Equal(value, second.Get(key));
	}
}
