using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
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
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IService>();

		var ips = new IPAddress[2]
		{
			new IPAddress(new byte[4] { 192, 168, 1, 102, }),
			new IPAddress(new byte[4] { 192, 168, 1, 217, }),
		};

		foreach (var ip in ips)
		{
			var lights = await sut.GetLightStatusAsync(ip).ToListAsync();

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
