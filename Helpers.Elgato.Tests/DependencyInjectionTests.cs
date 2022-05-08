using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class DependencyInjectionTests : IClassFixture<Fixtures.ConfigFixture>
{
	private readonly Fixtures.ConfigFixture _configFixture;

	public DependencyInjectionTests(Fixtures.ConfigFixture configFixture)
	{
		_configFixture = configFixture;
	}

	[Fact]
	public async Task Test1()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["Elgato:Scheme"] = Config.DefaultScheme,
					["Elgato:Port"] = Config.DefaultPort.ToString("D"),
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.JsonConfig<Config>(configuration.GetSection("Elgato"))
				.AddTransient<IService, Concrete.Service>()
				.AddHttpClient<IClient, Concrete.Client>()
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IService>();

		var ips = new[]
		{
			_configFixture.KeylightIPAddress,
			_configFixture.LightstripIPAddress,
		};

		foreach (var ip in ips)
		{
			var info = await sut.GetLightAsync(ip);

			Assert.NotNull(info);
		}
	}
}
