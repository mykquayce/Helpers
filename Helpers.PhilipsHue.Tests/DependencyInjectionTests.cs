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

	[Theory]
	[InlineData("wall right")]
	public async Task NetworkDiscoveryApiInjectionTests(string alias)
	{
		float brightness;
		{
			IServiceProvider serviceProvider;
			{
				var secrets = new Helpers.XUnitClassFixtures.UserSecretsFixture();
				T config<T>(string section) => secrets.Configuration.GetSection(section).Get<T>()!;

				var philipsHueConfig = config<Config>("philipshue")!;
				var identityConfig = config<Helpers.Identity.Config>("identity")!;
				var networkDiscoveryConfig = config<Helpers.NetworkDiscovery.Config>("networkdiscovery")!;

				serviceProvider = new ServiceCollection()
					.AddNetworkDiscovery(identityConfig, networkDiscoveryConfig)
					.AddPhilipsHue(philipsHueConfig, provider =>
					{
						var client = provider.GetRequiredService<Helpers.NetworkDiscovery.IClient>();
						(_, _, var ip, _, _) = client.ResolveAsync("philipshue").GetAwaiter().GetResult();
						return new UriBuilder("http", ip.ToString()).Uri;
					})
					.BuildServiceProvider();
			}

			var sut = serviceProvider.GetRequiredService<IService>();

			brightness = await sut.GetLightBrightnessAsync(alias);

			await ((ServiceProvider)serviceProvider).DisposeAsync();
		}

		Assert.InRange(brightness, 0f, 1f);
	}
}
