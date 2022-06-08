using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class DependencyInjectionTests : IClassFixture<Fixtures.ConfigFixture>
{
	private readonly Fixtures.ConfigFixture _configFixture;
	private readonly IConfiguration _configuration;

	public DependencyInjectionTests(Fixtures.ConfigFixture configFixture)
	{
		_configFixture = configFixture;
		_configuration = configFixture.Configuration;
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
					["EndPoints:IdentityServer"] = _configuration["identity:authority"],
					["EndPoints:NetworkDiscoveryApi"] = _configuration["NetworkDiscoveryApi"],
					["Identity:Authority"] = _configuration["identity:authority"],
					["Identity:ClientId"] = _configuration["identity:clientid"],
					["Identity:ClientSecret"] = _configuration["identity:clientsecret"],
					["Identity:Scope"] = _configuration["identity:scope"],
					["Aliases:keylight"] = _configuration["Elgato:Keylight:PhysicalAddress"],
					["Aliases:lightstrip"] = _configuration["Elgato:Lightstrip:PhysicalAddress"],
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
				.AddHttpClient<IClient, Concrete.Client>("ElgatoClient")
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.AddTransient<Helpers.NetworkDiscoveryApi.IAliasResolverService, Helpers.NetworkDiscoveryApi.Concrete.AliasResolverService>()
				.AddAliasResolver(configuration)
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IService>();

		foreach (var alias in new[] { "keylight", "lightstrip", })
		{
			var lights = await sut.GetLightStatusAsync(alias).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);
		}
	}

	[Theory]
	[InlineData(0, 1, 2, 3, 4, 5)]
	public void DataBindingOfArraysTests(params int[] values)
	{
		// Arrange
		var initialData = values
			.ToDictionary(i => "values:" + i, i => (string?)i.ToString());

		IConfiguration configuration;
		{
			configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();
		}

		// Act
		IReadOnlyCollection<int> actual = new List<int>();
		configuration.GetSection("values").Bind(actual);

		// Assert
		Assert.NotEmpty(actual);
		Assert.Equal(values, actual);
	}
}
