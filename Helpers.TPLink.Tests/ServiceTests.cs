using System.Net;

namespace Helpers.TPLink.Tests;

[Collection(nameof(NonParallelCollection))]
public class ServiceTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IService _service;

	public ServiceTests(Fixtures.Fixture fixture)
	{
		_service = fixture.Service;
	}

	[Fact]
	public async Task DiscoveryTests()
	{
		var ips = await GetDevicesIPAddressesAsync().ToArrayAsync();
		Assert.NotEmpty(ips);
		Assert.All(ips, ip => Assert.NotEqual(IPAddress.None, ip));
	}

	private IAsyncEnumerable<IPAddress> GetDevicesIPAddressesAsync()
		=> _service.DiscoverAsync().Select(tuple => tuple.Item2);

	[Fact]
	public async Task GetRealtimeDataTests()
	{
		await foreach (var ip in GetDevicesIPAddressesAsync())
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
			var data = await _service.GetRealtimeDataAsync(ip, cts.Token).ToArrayAsync(cts.Token);

			Assert.Single(data);
			var (amps, volts, watts) = data[0];

			Assert.InRange(volts, 230, 255);
			if (amps > 0)
			{
				Assert.InRange(amps, .001, 1_000);
				Assert.InRange(watts, .1, 10);
			}
			else
			{
				Assert.Equal(0, amps);
				Assert.Equal(0, watts);
			}
		}
	}

	[Fact]
	public async Task GetRealtimeDataTests_NoCancellationTokens()
	{
		await foreach (var ip in GetDevicesIPAddressesAsync())
		{
			var data = await _service.GetRealtimeDataAsync(ip).ToArrayAsync();

			Assert.Single(data);
			var (amps, volts, watts) = data[0];

			Assert.InRange(volts, 230, 255);
			if (amps > 0)
			{
				Assert.InRange(amps, .001, 1_000);
				Assert.InRange(watts, .1, 10);
			}
			else
			{
				Assert.Equal(0, amps);
				Assert.Equal(0, watts);
			}
		}
	}

	[Fact]
	public async Task GetStateTests()
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);

		var devices = await _service.DiscoverAsync().ToListAsync(cts.Token);

		Assert.NotEmpty(devices);
		Assert.All(devices, tuple => Assert.NotEqual(default, tuple));

		foreach ((_, var ip, _) in devices)
		{
			await _service.GetStateAsync(ip, cts.Token).FirstAsync(cts.Token);
		}
	}
}
