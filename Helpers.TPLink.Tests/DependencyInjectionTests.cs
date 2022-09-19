using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.TPLink.Tests;

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
	[InlineData(1)]
	public void ConfigTests(ushort port)
	{
		var config = new Config(port);

		var provider = new ServiceCollection()
			.AddSingleton(config)
			.AddTransient<ITPLinkClient, Concrete.TPLinkClient>()
			.BuildServiceProvider();

		var sut = provider.GetService<ITPLinkClient>();

		Assert.NotNull(sut);
	}
}
