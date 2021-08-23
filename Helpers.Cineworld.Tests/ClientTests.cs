using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Cineworld.Tests;

public class ClientTests
{
#pragma warning disable xUnit1004 // Test methods should not be skipped
	[Fact(Skip = "calls 3rd party api")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
	public async Task CacheTests()
	{
		var config = Concrete.Client.Config.Defaults with { CacheExpiration = TimeSpan.FromSeconds(10), };
		using var httpClient = new HttpClient { BaseAddress = config.BaseAddressUri, };
		var xmlSerializerFactory = new XmlSerializerFactory();
		using var memoryCache = new MemoryCache(new MemoryCacheOptions());

		IClient client = new Concrete.Client(config, httpClient, xmlSerializerFactory, memoryCache);

		var stopwatch = Stopwatch.StartNew();
		var times = new List<TimeSpan>();

		using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

		while (!cts.IsCancellationRequested)
		{
			try
			{
				stopwatch.Restart();
				await client.GetAllPerformancesAsync(cts.Token);
				times.Add(stopwatch.Elapsed);
				await Task.Delay(millisecondsDelay: 1_000, cts.Token);
			}
			catch (TaskCanceledException) { break; }
		}

		stopwatch.Stop();

		// Assert there's many requests
		Assert.InRange(times.Count, 2, 20);
		// Only two took more than a millisecond (i.e. the first, and when the cache expired)
		Assert.Equal(2, times.Count(ts => ts.TotalMilliseconds > 1));
	}

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
