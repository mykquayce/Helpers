using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Tests;

[Collection(nameof(NonParallelCollection))]
public class ServiceTests(Fixtures.Fixture fixture) : IClassFixture<Fixtures.Fixture>
{
	private static readonly IPEndPoint _broadcastEndPoint = IPEndPoint.Parse("192.168.1.255:" + Constants.Port);
	private readonly IService _service = fixture.Service;

	[Fact]
	public async Task DiscoveryTests()
	{
		var endPoints = await GetDevicesIPAddressesAsync().ToArrayAsync();
		Assert.NotEmpty(endPoints);
		Assert.DoesNotContain(null, endPoints);
	}


	private async IAsyncEnumerable<IPEndPoint> GetDevicesIPAddressesAsync()
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
		var devices = _service.DiscoverAsync(_broadcastEndPoint, cts.Token);
		await foreach ((_, var endPoint, _) in devices)
		{
			yield return endPoint;
		}
	}

	[Fact]
	public async Task GetRealtimeDataTests2()
	{
		await foreach (var endPoint in GetDevicesIPAddressesAsync())
		{
			var data = await _service.GetRealtimeDataAsync(endPoint);
			Assert.NotEqual(default, data);
		}
	}

	[Fact]
	public async Task GetRealtimeDataTests()
	{
		await foreach (var endPoint in GetDevicesIPAddressesAsync())
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
			var data = await _service.GetRealtimeDataAsync(endPoint, cts.Token);

			Assert.NotEqual(default, data);
			var (amps, volts, watts) = data;

			Assert.InRange(volts, 220, 255);
			if (amps > 0)
			{
				Assert.InRange(amps, .001, 1_000);
				Assert.InRange(watts, .1, 50);
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
		await foreach (var endPoint in GetDevicesIPAddressesAsync())
		{
			var data = await _service.GetRealtimeDataAsync(endPoint);

			var (amps, volts, watts) = data;

			Assert.InRange(volts, 230, 255);
			if (amps > 0)
			{
				Assert.InRange(amps, .001, 1_000);
				Assert.InRange(watts, .1, 50);
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
		ValueTuple<string, IPEndPoint, PhysicalAddress>[] devices;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
			devices = await _service.DiscoverAsync(_broadcastEndPoint).ToArrayAsync(cts.Token);
		}

		Assert.NotEmpty(devices);
		Assert.DoesNotContain(default, devices);

		foreach ((_, var endPoint, _) in devices)
		{
			await _service.GetStateAsync(endPoint);
		}
	}

	[Fact]
	public async Task GetSystemInfoTests()
	{
		await foreach (var endPoint in GetDevicesIPAddressesAsync())
		{
			var info = await _service.GetSystemInfoAsync(endPoint);

			Assert.NotEqual(default, info);
			Assert.NotEqual(default, info.alias);
		}
	}

	public class Fixture
	{
		public Fixture()
		{
			BroadcastAddresses = Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses()
				.Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				.Select(a => a.GetBroadcastAddress()).ToArray();
		}

		public IReadOnlyCollection<IPAddress> BroadcastAddresses { get; }
	}
}
