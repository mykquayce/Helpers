using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.NetworkDiscovery.Tests;

public class ClientTests : IClassFixture<Fixtures.ConfigFixture>
{
	private static readonly DateTime _now = DateTime.UtcNow;
	private readonly Helpers.Identity.Config _identityConfig;
	private readonly Config _config;

	public ClientTests(Fixtures.ConfigFixture fixture)
	{
		_identityConfig = fixture.IdentityConfig;
		_config = fixture.Config;
	}

	[Theory]
	[InlineData("28ee52eb0aa4")]
	[InlineData("vr front")]
	public async Task ResolveTests(object key)
	{
		Helpers.Networking.Models.DhcpLease lease;
		{
			using var httpHandler = new HttpClientHandler { AllowAutoRedirect = false, };
			using var identityHttpClient = new HttpClient(httpHandler) { BaseAddress = _identityConfig.Authority, };
			using var httpClient = new HttpClient(httpHandler) { BaseAddress = _config.BaseAddress, };
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());

			var identityClient = new Helpers.Identity.Clients.Concrete.IdentityClient(_identityConfig, identityHttpClient, memoryCache);

			var sut = new Concrete.Client(httpClient, identityClient);

			using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);
			lease = await sut.ResolveAsync(key, cts.Token);
		}

		Assert.NotNull(lease);
		Assert.InRange(lease.Expiration, _now, _now.AddDays(1));
	}

	[Theory]
	[InlineData("28ee52eb0aa4")]
	[InlineData("vr front")]
	public async Task DependencyInjectionTests(object key)
	{
		Helpers.Networking.Models.DhcpLease lease;
		{
			IServiceProvider serviceProvider;
			{
				var secrets = new XUnitClassFixtures.UserSecretsFixture();
				var identityConfig = secrets.GetSection<Identity.Config>(section: "identity");
				var config = secrets.GetSection<Config>(section: "networkdiscovery");

				serviceProvider = new ServiceCollection()
					.AddNetworkDiscovery(identityConfig, config)
					.BuildServiceProvider();
			}

			var client = serviceProvider.GetRequiredService<IClient>();

			using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);
			lease = await client.ResolveAsync(key, cts.Token);

			await ((ServiceProvider)serviceProvider).DisposeAsync();
		}

		Assert.NotNull(lease);
		Assert.InRange(lease.Expiration, _now, _now.AddDays(1));
	}
}
