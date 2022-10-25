using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class DependencyInjectionTests : IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IReadOnlyCollection<IPAddress> _ipAddresses;

	public DependencyInjectionTests(Fixtures.ConfigFixture configFixture)
	{
		_ipAddresses = configFixture.Configuration.GetSection("elgato:ipaddresses")
			.Get<string[]>()!
			.Select(IPAddress.Parse)
			.ToArray()
			.AsReadOnly();
	}

	[Theory]
	[InlineData("http", 9_123)]
	public async Task ValuesTests(string scheme, int port)
	{
		using var serviceProvider = new ServiceCollection()
			.AddElgato(scheme, port)
			.BuildServiceProvider();

		await TestServiceProvider(serviceProvider);
	}

	[Theory]
	[InlineData("http", 9_123)]
	public async Task ConfigTests(string scheme, int port)
	{
		var config = new Config(scheme, port);

		using var serviceProvider = new ServiceCollection()
			.AddElgato(config)
			.BuildServiceProvider();

		await TestServiceProvider(serviceProvider);
	}

	[Theory]
	[InlineData("http", 9_123)]
	public async Task ConfigurationTests(string scheme, int port)
	{
		IConfiguration configuration;
		{
			var initialData = new Dictionary<string, string?>
			{
				[nameof(scheme)] = scheme,
				[nameof(port)] = port.ToString(),
			};

			configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();
		}

		using var serviceProvider = new ServiceCollection()
			.AddElgato(configuration)
			.BuildServiceProvider();

		await TestServiceProvider(serviceProvider);
	}

	private async Task TestServiceProvider(IServiceProvider serviceProvider)
	{
		var sut = serviceProvider.GetRequiredService<Helpers.Elgato.IService>();

		foreach (var ip in _ipAddresses)
		{
			var lights = await sut.GetLightStatusAsync(ip).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);
			Assert.NotEqual(default, lights);

			foreach (var (on, brightness) in lights)
			{
				Assert.InRange(brightness, 0, 1);
			}
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
