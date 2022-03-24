using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.Identity.Tests;

public class DependencyInjectionTests
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData("https://identityserver", "client", "secret", "api1")]
	public async Task Test1(string authority, string clientId, string clientSecret, string scope)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["Authority"] = authority,
					["ClientId"] = clientId,
					["ClientSecret"] = clientSecret,
					["Scope"] = scope,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.Configure<Config>(configuration)
				.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()))
				.AddHttpClient<Identity.Clients.IIdentityClient, Identity.Clients.Concrete.IdentityClient>((serviceProvider, client) =>
				{
					var options = serviceProvider.GetRequiredService<IOptions<Config>>();
					client.BaseAddress = options.Value.Authority;
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.AddHttpClient<Clients.ISecureWebClient, Clients.Concrete.SecureWebClient>((serviceProvider, client) =>
				{
					client.BaseAddress = new Uri("https://api:6001");
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<Clients.ISecureWebClient>();

		var json = await sut.GetStringAsync("/identity");

		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.StartsWith("[", json);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData("https://identityserver", "client", "secret", "api1")]
	public async Task Test2(string authority, string clientId, string clientSecret, string scope)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					[nameof(authority)] = authority,
					[nameof(clientId)] = clientId,
					[nameof(clientSecret)] = clientSecret,
					[nameof(scope)] = scope,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddIdentityClient(configuration)
				.AddHttpClient<Clients.ISecureWebClient, Clients.Concrete.SecureWebClient>(client =>
				{
					client.BaseAddress = new Uri("https://api:6001");
				})
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
				.Services
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<Clients.ISecureWebClient>();

		var json = await sut.GetStringAsync("/identity");

		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.StartsWith("[", json);
	}
}
