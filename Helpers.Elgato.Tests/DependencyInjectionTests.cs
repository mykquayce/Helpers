using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Helpers.Elgato.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public async Task Test1()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var configFixture = new XUnitClassFixtures.UserSecretsFixture();

				var initialData = new Dictionary<string, string?>
				{
					["NetworkDiscoveryApi"] = configFixture["NetworkDiscoveryApi"],
					["Elgato:Scheme"] = Config.DefaultScheme,
					["Elgato:PhysicaAddress"] = configFixture["Elgato:PhysicalAddress"],
					["Elgato:Port"] = Config.DefaultPort.ToString("D"),
					["Identity:Authority"] = configFixture["Identity:Authority"],
					["Identity:ClientId"] = configFixture["Identity:ClientId"],
					["Identity:ClientSecret"] = configFixture["Identity:ClientSecret"],
					["Identity:Scope"] = configFixture["Identity:Scope"],
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.JsonConfig<Config>(configuration.GetSection("Elgato"))
				.AddIdentityClient(configuration.GetSection("Identity"))
				.AddHttpClient<Helpers.NetworkDiscoveryApi.IClient, Helpers.NetworkDiscoveryApi.Concrete.SecureClient>((provider, client) =>
				{
					client.BaseAddress = new Uri(configuration["NetworkDiscoveryApi"]);
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.AddHttpClient<IElgatoClient, Concrete.ElgatoClient>((provider, client) =>
				{
					var config = provider.GetRequiredService<IOptions<Config>>().Value;
					var networkDiscoveryClient = provider.GetRequiredService<Helpers.NetworkDiscoveryApi.IClient>();

					(_, _, var ip, _, _) = networkDiscoveryClient.GetLeasesAsync()
						.FirstAsync(t => t.physicalAddress.Equals(config.PhysicalAddress))
						.AsTask()
						.GetAwaiter().GetResult();

					client.BaseAddress = new Uri($"{config.Scheme}://{ip}:{config.Port}");
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IElgatoClient>();

		var info = await sut.GetAccessoryInfoAsync();

		Assert.NotNull(info);
		Assert.NotNull(info.displayName);
	}
}
