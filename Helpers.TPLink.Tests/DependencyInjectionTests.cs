using Microsoft.Extensions.DependencyInjection;

namespace Helpers.TPLink.Tests;

[Collection(nameof(NonParallelCollection))]
public class DependencyInjectionTests
{
	[Fact]
	public async Task Test1()
	{
		using var provider = new ServiceCollection()
			.AddTPLink()
			.BuildServiceProvider();

		var discoveryClient = provider.GetRequiredService<IDiscoveryClient>();
		var client = provider.GetRequiredService<IClient>();
		var service = provider.GetRequiredService<IService>();

		var devices = await discoveryClient.DiscoverAsync().ToListAsync();

		Assert.NotEmpty(devices);

		foreach ((_, var ip, _) in devices)
		{
			var (amps, volts, watts) = await service.GetRealtimeDataAsync(ip).FirstAsync();

			Assert.InRange(amps, 0, 13);
			Assert.InRange(volts, 230, 250);
			Assert.InRange(watts, 0, 1_000);
		}
	}
}
