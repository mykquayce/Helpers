using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.TPLink.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class DependencyInjectionTests
{
	[Theory]
	[InlineData(1)]
	public void OptionsTests(ushort port)
	{
		var config = new Config(port);
		var options = Options.Create(config);

		var provider = new ServiceCollection()
			.AddSingleton(options)
			.AddTransient<ITPLinkClient, Concrete.TPLinkClient>()
			.BuildServiceProvider();

		var sut = provider.GetService<ITPLinkClient>();

		Assert.NotNull(sut);
	}

	[Theory]
	[InlineData(9_999)]
	public async Task Test1(ushort port)
	{
		using var provider = new ServiceCollection()
			.AddTPLink(port)
			.BuildServiceProvider();

		var client = provider.GetRequiredService<ITPLinkClient>();
		var service = provider.GetRequiredService<ITPLinkService>();

		var devices = await client.DiscoverAsync().ToListAsync();

		Assert.NotEmpty(devices);

		foreach (var (alias, ip, mac) in devices)
		{
			Models.SystemInfo systemInfo;

			systemInfo = await service.GetSystemInfoAsync(alias);
			Assert.NotNull(systemInfo);

			systemInfo = await service.GetSystemInfoAsync(ip);
			Assert.NotNull(systemInfo);

			systemInfo = await service.GetSystemInfoAsync(mac);
			Assert.NotNull(systemInfo);
		}
	}

	[Theory]
	[InlineData(100)]
	public async Task MemoryCacheLoopTests(int timeout)
	{
		using var provider = new ServiceCollection()
			.AddTPLink(Config.Defaults)
			.BuildServiceProvider();

		var services = provider.GetServices<IMemoryCache>().ToList();

		var tasks = new Task[2]
			{
				Task.Run(provider.GetRequiredService<IMemoryCache>),
				Task.Delay(timeout),
			};

		var task = await Task.WhenAny(tasks);

		_ = Assert.IsType<Task<IMemoryCache>>(task);
	}
}
