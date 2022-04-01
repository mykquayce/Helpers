using System.Diagnostics;

namespace Helpers.GlobalCache.Tests;

[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
public class CacheTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _sut;

	public CacheTests(Fixtures.ServiceFixture fixture)
	{
		_sut = fixture.Service;
	}

	[Theory]
	[InlineData(10, "GlobalCache_000C1E059CAD")]
	public async Task ConnectionCacheTests(int count, string uuid)
	{
		var times = new List<long>();
		while (count-- > 0)
		{
			var stopwatch = Stopwatch.StartNew();
			await _sut.ConnectAsync(uuid);
			stopwatch.Stop();
			times.Add(stopwatch.ElapsedTicks);
			await Task.Delay(millisecondsDelay: 500);
		}

		Assert.InRange(times[0], 10_000_000, 100_000_000); // first between 1 and 10 seconds.
		Assert.All(times.Skip(1), t => Assert.InRange(t, 10, 10_000)); // remainder between 1 micro- and 1 milli-second.
	}
}
