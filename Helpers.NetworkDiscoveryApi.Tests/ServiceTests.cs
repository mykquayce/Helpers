using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _service;

	public ServiceTests(Fixtures.ServiceFixture serviceFixture)
	{
		_service = serviceFixture.Service;
	}

	[Theory]
	[InlineData(10, "3C:6A:9D:14:D7:65")]
	public async Task CacheTests(int count, string physicalAddressString)
	{
		// Arrange
		var times = new List<double>();
		var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
		var stopwatch = Stopwatch.StartNew();

		while (count-- > 0)
		{
			// Act
			await _service.GetLeaseAsync(physicalAddress);
			times.Add(stopwatch.Elapsed.TotalMilliseconds);
			stopwatch.Restart();
		}

		stopwatch.Stop();

		var first = times.First();
		var remainder = times.Skip(1).ToArray();

		// Assert the first is more than 100ms
		Assert.InRange(first, 100, 10_000);
		// Assert the rest are less than a millisecond
		Assert.All(remainder, t => Assert.InRange(t, 0, 1));
	}

	[Theory]
	[InlineData("3C:6A:9D:14:D7:65")]
	public async Task GetLeasesPopulatesTheCacheTests(string physicalAddressString)
	{
		// Arrange
		double first, second;
		var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
		var stopwatch = Stopwatch.StartNew();

		// Act
		await _service.GetLeasesAsync().ToListAsync();

		// Arrange
		first = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Restart();

		// Act
		await _service.GetLeaseAsync(physicalAddress);

		// Arrange
		second = stopwatch.Elapsed.TotalMilliseconds;
		stopwatch.Stop();

		// Assert the first is more than 100ms
		Assert.InRange(first, 100, 10_000);
		// Assert the second is less than a millisecond
		Assert.InRange(second, 0, 1);
	}

	[Theory]
	[InlineData("keylight")]
	[InlineData("lightstrip")]
	public async Task GetLeaseByAliasTests(string alias)
	{
		var today = DateTime.Today;
		DateTime expiration;
		PhysicalAddress mac;
		IPAddress ip;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);
			(expiration, mac, ip, _, _) = await _service.GetLeaseAsync(alias, cts.Token);
		}
		Assert.InRange(expiration, today, today.AddDays(1));
		Assert.NotNull(mac);
		Assert.NotEqual(PhysicalAddress.None, mac);
		Assert.NotNull(ip);
		Assert.NotEqual(IPAddress.None, ip);
	}
}
