using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientDependencyInjectionTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly IConfiguration _configuration;

	public ClientDependencyInjectionTests(Fixtures.ConfigurationFixture configurationFixture)
	{
		var initialData = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
		{
			["EndPoints:IdentityServer"] = configurationFixture.Authority.OriginalString,
			["EndPoints:NetworkDiscoveryApi"] = configurationFixture.BaseAddress.OriginalString,
			["Identity:Authority"] = configurationFixture.Authority.OriginalString,
			["Identity:ClientId"] = configurationFixture.ClientId,
			["Identity:ClientSecret"] = configurationFixture.ClientSecret,
			["Identity:Scope"] = configurationFixture.Scope,
		};

		_configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();
	}

	[Theory]
	[InlineData(5_000)]
	public async Task DependencyInjectionTests(int millisecondsDelay)
	{
		var serviceProvider = new ServiceCollection()
			.AddNetworkDiscoveryApi(_configuration)
			.BuildServiceProvider();

		var client = serviceProvider.GetService<IClient>();

		Assert.NotNull(client);
		ICollection<Models.DhcpResponseObject> leases;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay);
			leases = await client!.GetLeasesAsync(cts.Token).ToListAsync(cts.Token);
		}

		Assert.NotNull(leases);
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);
		foreach (var (expiration, ip, mac, hostName, identifier) in leases)
		{
			Assert.NotEqual(default, expiration);
			Assert.NotEqual(default, mac);
			Assert.NotEqual(default, ip);
			Assert.NotEqual(string.Empty, hostName);
			Assert.NotEqual(string.Empty, identifier);
		}
	}
}
