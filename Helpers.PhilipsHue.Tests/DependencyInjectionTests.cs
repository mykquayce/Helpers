using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public void ConfigurationTests()
	{
		var initialData = new Dictionary<string, string?>
		{
			[nameof(Config.DiscoveryEndPoint)] = Config.DefaultDiscoveryEndPoint.ToString(),
			[nameof(Config.Username)] = Config.DefaultUsername,
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();

		using var serviceProvider = new ServiceCollection()
			.AddPhilipsHue(configuration)
			.BuildServiceProvider();

		serviceProvider.GetRequiredService<IService>();
	}

	[Fact]
	public void ArgumentsTests()
	{
		using var serviceProvider = new ServiceCollection()
			.AddPhilipsHue(Config.DefaultUsername, Config.DefaultDiscoveryEndPoint)
			.BuildServiceProvider();

		serviceProvider.GetRequiredService<IService>();
	}

	[Fact]
	public void ObjectTests()
	{
		var config = Config.Defaults;

		using var serviceProvider = new ServiceCollection()
			.AddPhilipsHue(config)
			.BuildServiceProvider();

		serviceProvider.GetRequiredService<IService>();
	}

	[Fact]
	public void NothingTests()
	{
		using var serviceProvider = new ServiceCollection()
			.AddSingleton(Options.Create(Config.Defaults))
			.AddPhilipsHue()
			.BuildServiceProvider();

		serviceProvider.GetRequiredService<IService>();
	}

	[Theory]
	[InlineData("username", "https://discovery.meethue.com/")]
	public void ClientHasBaseAddressFromDiscoveryClientTests(string username, string discoveryEndPoint)
	{
		using var serviceProvider = new ServiceCollection()
			.AddPhilipsHue(username: username, discoveryEndPoint: new Uri(discoveryEndPoint))
			.BuildServiceProvider();

		serviceProvider.GetRequiredService<IClient>();
	}
}
