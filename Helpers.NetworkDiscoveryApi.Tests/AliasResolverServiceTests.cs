using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

public class AliasResolverServiceTests
{
	[Theory]
	[InlineData("keylight", "3c6a9d14d765", "lightstrip", "3c6a9d187071")]
	public async Task Test1(params string[] aliasesArray)
	{
		var aliases = new Aliases(aliasesArray.Chunk(size: 2).ToDictionary(ss => ss[0], ss => ss[1]));

		using var httpClient = new HttpClient { BaseAddress = new Uri("https://networkdiscovery"), };
		var authority = new Uri("https://identityserver");
		using var identityServerHttpClient = new HttpClient { BaseAddress = authority, };
		using var memoryCache = new MemoryCache(new MemoryCacheOptions());
		Identity.Clients.IIdentityClient identityClient;
		{
			Identity.Config config;
			{
				var clientId = "elgatoapi";
				var clientSecret = "8556e52c6ab90d042bb83b3f0c8894498beeb65cf908f519a2152aceb131d3ee";
				var scope = "networkdiscovery";
				config = new Identity.Config(authority, clientId, clientSecret, scope);
			}

			identityClient = new Identity.Clients.Concrete.IdentityClient(config, identityServerHttpClient, memoryCache);
		}

		IClient client = new Concrete.SecureClient(httpClient, identityClient);

		IService service = new Concrete.Service(client, memoryCache);

		var sut = new Concrete.AliasResolverService(aliases, service);

		foreach (var alias in aliases.Keys)
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 3_000);

			var ip = await sut.ResolveAsync(alias, cts.Token);

			Assert.NotNull(ip);
			Assert.NotEqual(IPAddress.None, ip);
		}
	}

	[Fact]
	public async Task DependencyInjectionTests()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string>
				{
					["EndPoints:IdentityServer"] = "https://identityserver",
					["EndPoints:NetworkDiscoveryApi"] = "https://networkdiscovery",
					["Identity:Authority"] = "https://identityserver",
					["Identity:ClientId"] = "elgatoapi",
					["Identity:ClientSecret"] = "8556e52c6ab90d042bb83b3f0c8894498beeb65cf908f519a2152aceb131d3ee",
					["Identity:Scope"] = "networkdiscovery",
					["Aliases:keylight"] = "3c6a9d14d765",
					["Aliases:lightstrip"] = "3c6a9d187071",
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddTransient<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()))
				// identity client
				.Configure<Identity.Config>(configuration.GetSection("Identity"))
				.AddHttpClient<Identity.Clients.IIdentityClient, Identity.Clients.Concrete.IdentityClient>((provider, client) =>
				{
					var options = provider.GetRequiredService<IOptions<EndPoints>>();
					var endPoints = options.Value;
					var baseAddress = endPoints.IdentityServer;
					client.BaseAddress = baseAddress;
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				// network discovery client
				.JsonConfig<EndPoints>(configuration.GetSection("EndPoints"))
				.AddHttpClient<IClient, Concrete.SecureClient>((provider, client) =>
				{
					var options = provider.GetRequiredService<IOptions<EndPoints>>();
					var endPoints = options.Value;
					var baseAddress = endPoints.NetworkDiscoveryApi;
					client.BaseAddress = baseAddress;
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				// network discovery service
				.AddTransient<IService, Concrete.Service>()
				// alias resolver service
				.Configure<Aliases>(configuration.GetSection("Aliases"))
				.AddTransient<IAliasResolverService, Concrete.AliasResolverService>()
				// build service provider
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IAliasResolverService>();

		var ip = await sut.ResolveAsync("keylight");

		Assert.NotNull(ip);
		Assert.NotEqual(IPAddress.None, ip);
	}

	[Fact]
	public async Task ExtensionMethodTests()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string>
				{
					["EndPoints:IdentityServer"] = "https://identityserver",
					["EndPoints:NetworkDiscoveryApi"] = "https://networkdiscovery",
					["Identity:Authority"] = "https://identityserver",
					["Identity:ClientId"] = "elgatoapi",
					["Identity:ClientSecret"] = "8556e52c6ab90d042bb83b3f0c8894498beeb65cf908f519a2152aceb131d3ee",
					["Identity:Scope"] = "networkdiscovery",
					["Aliases:keylight"] = "3c6a9d14d765",
					["Aliases:lightstrip"] = "3c6a9d187071",
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddAliasResolver(configuration)
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<IAliasResolverService>();

		var ip = await sut.ResolveAsync("keylight");

		Assert.NotNull(ip);
		Assert.NotEqual(IPAddress.None, ip);
	}
}
