using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Tests;

[Collection(nameof(NonParallelCollection))]
public class ClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut;
	private readonly IDiscoveryClient _discoveryClient;

	public ClientTests(Fixtures.Fixture fixture)
	{
		_sut = fixture.Client;
		_discoveryClient = fixture.DiscoveryClient;
	}

	[Fact]
	public async Task DiscoveryTests()
	{
		// Arrange
		using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);

		// Act
		var devices = await _discoveryClient.DiscoverAsync(cts.Token).ToArrayAsync(cts.Token);

		// Assert
		Assert.NotEmpty(devices);
		Assert.All(devices, device => Assert.NotEqual(default, device));
	}

	private IAsyncEnumerable<IPAddress> GetDevicesIPAddressesAsync()
		=> _discoveryClient.DiscoverAsync().Select(tuple => tuple.Item2);

	[Fact]
	public async Task GetRealtimeDataTest()
	{
		// Arrange
		var ips = GetDevicesIPAddressesAsync();

		await foreach (var ip in ips)
		{
			// Arrange
			using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);

			// Act
			var data = await _sut.GetRealtimeDataAsync(ip, cts.Token).ToArrayAsync(cts.Token);

			Assert.Single(data);
			var (milliamps, millivolts, milliwatts) = data[0];

			// Assert
			Assert.InRange(millivolts, 230_000, 255_000);

			if (milliamps > 0)
			{
				Assert.InRange(milliamps, 1, 1_000);
				Assert.InRange(milliwatts, 100, 10_000);
			}
			else
			{
				Assert.Equal(0, milliamps);
				Assert.Equal(0, milliwatts);
			}
		}
	}

	[Fact]
	public async Task GetSystemInfoTests()
	{
		// Arrange
		var ips = GetDevicesIPAddressesAsync();

		await foreach (var ip in ips)
		{
			// Arrange
			using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);

			// Act
			var infos = await _sut.GetSystemInfoAsync(ip, cts.Token).ToArrayAsync(cts.Token);

			Assert.Single(infos);
			var (alias, mac, model, relay_state) = infos[0];

			// Assert
			Assert.NotNull(alias);
			Assert.NotEmpty(alias);
			Assert.NotEqual(PhysicalAddress.None, mac);
			Assert.NotNull(model);
			Assert.NotEmpty(model);
			Assert.InRange(relay_state, 0, 1);
		}
	}

	[Fact]
	public async Task GetSystemInfoTests_NoCancellationTokens()
	{
		// Arrange
		var ips = GetDevicesIPAddressesAsync();

		await foreach (var ip in ips)
		{
			// Act
			var infos = await _sut.GetSystemInfoAsync(ip).ToArrayAsync();

			Assert.Single(infos);
			var (alias, mac, model, relay_state) = infos[0];

			// Assert
			Assert.NotNull(alias);
			Assert.NotEmpty(alias);
			Assert.NotEqual(PhysicalAddress.None, mac);
			Assert.NotNull(model);
			Assert.NotEmpty(model);
			Assert.InRange(relay_state, 0, 1);
		}
	}

	[Fact]
	public async Task GetStateTests()
	{
		var ips = GetDevicesIPAddressesAsync();
		await foreach (var ip in ips)
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
			var states = await _sut.GetStateAsync(ip, cts.Token).ToArrayAsync(cts.Token);
			Assert.Single(states);
		}
	}
}
