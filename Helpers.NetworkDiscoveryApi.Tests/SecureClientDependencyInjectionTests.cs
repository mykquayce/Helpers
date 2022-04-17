using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class SecureClientDependencyInjectionTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly Uri _authority, _baseAddress;
	private readonly string _clientId, _clientSecret, _scope;

	public SecureClientDependencyInjectionTests(Fixtures.ConfigurationFixture configurationFixture)
	{
		_authority = configurationFixture.Authority;
		_baseAddress = configurationFixture.BaseAddress;
		_clientId = configurationFixture.ClientId;
		_clientSecret = configurationFixture.ClientSecret;
		_scope = configurationFixture.Scope;
	}

	[Theory]
	[InlineData(5_000)]
	public async Task Test1(int millisecondsDelay)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["BaseAddress"] = _baseAddress.OriginalString,
					["Identity:Authority"] = _authority.OriginalString,
					["Identity:ClientId"] = _clientId,
					["Identity:ClientSecret"] = _clientSecret,
					["Identity:Scope"] = _scope,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.JsonConfig<Identity.Config>(configuration.GetSection("Identity"))
				.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
				.AddHttpClient<Identity.Clients.IIdentityClient, Identity.Clients.Concrete.IdentityClient>((serviceProvider, client) =>
				{
					var config = serviceProvider.GetRequiredService<IOptions<Identity.Config>>().Value;
					client.BaseAddress = config.Authority;
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.AddHttpClient<IClient, Concrete.SecureClient>(client =>
				{
					client.BaseAddress = new Uri(configuration["BaseAddress"]);
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.BuildServiceProvider();
		}

		var client = serviceProvider.GetRequiredService<IClient>();

		ICollection<Models.DhcpResponseObject> leases;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay);
			leases = await client.GetLeasesAsync(cts.Token).ToListAsync(cts.Token);
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
