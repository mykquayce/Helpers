using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientDependencyInjectionTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly Uri _baseAddress;

	public ClientDependencyInjectionTests(Fixtures.ConfigurationFixture configurationFixture)
	{
		_baseAddress = configurationFixture.BaseAddress;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "non-authorized communications no longer allowed")]
	[Theory(Skip = "non-authorized communications no longer allowed")]
	[InlineData(5_000)]
	public async Task DependencyInjectionTests(int millisecondsDelay)
	{
		var serviceProvider = new ServiceCollection()
			.AddHttpClient<IClient, Concrete.Client>(httpClient =>
			{
				httpClient.BaseAddress = _baseAddress;
			})
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new HttpClientHandler { AllowAutoRedirect = false, };
			})
			.Services
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
