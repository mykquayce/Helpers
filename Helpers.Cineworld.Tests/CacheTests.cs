using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Helpers.Cineworld.Tests;

public class CacheTests
{
	[Theory]
	[InlineData("nreasthianhsret")]
	public void ExpiryTests(string key)
	{
		using IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

		var times = new List<DateTime>();

		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		while (!cts.IsCancellationRequested)
		{
			if (!cache.TryGetValue(key, out DateTime time))
			{
				time = DateTime.UtcNow;

				var a = new CancellationTokenSource(millisecondsDelay: 1_000);
				var changeToken = new CancellationChangeToken(a.Token);

				var options = new MemoryCacheEntryOptions()
					.AddExpirationToken(changeToken);

				cache.Set(key, time, options);
			}

			times.Add(time);
			Thread.Sleep(millisecondsTimeout: 200);
		}

		var grouped = times.GroupBy(ts => ts).ToList();

		Assert.InRange(grouped.Count, 9, 11);
		Assert.All(grouped, g => Assert.InRange(g.Count(), 1, 6));
	}

	[Theory]
	[InlineData("00:10:00", 600_000)]
	public void TimeSpanParseTests(string input, int expectedMilliseconds)
	{
		var actual = TimeSpan.Parse(input);
		Assert.Equal(expectedMilliseconds, actual.TotalMilliseconds);
	}
}
