using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, IConfiguration configuration, GetBridgeBaseAddressDelegate? getBridgeBaseAddress = null)
	{
		Guard.Argument(configuration["username"]!)
			.NotNull().NotEmpty().NotWhiteSpace();

		return services
			.Configure<Helpers.PhilipsHue.Config>(configuration)
			.AddPhilipsHue(getBridgeBaseAddress);
	}

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, string username, Uri discoveryEndPoint, GetBridgeBaseAddressDelegate? getBridgeBaseAddress = null)
	{
		Guard.Argument(username).NotNull().NotEmpty().NotWhiteSpace();

		var config = new Helpers.PhilipsHue.Config(username, discoveryEndPoint);
		return services
			.AddPhilipsHue(config, getBridgeBaseAddress);
	}

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, Helpers.PhilipsHue.Config config, GetBridgeBaseAddressDelegate? getBridgeBaseAddress = null)
	{
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddPhilipsHue(getBridgeBaseAddress);
	}

	public delegate Uri GetBridgeBaseAddressDelegate(IServiceProvider serviceProvider);

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, GetBridgeBaseAddressDelegate? getBridgeBaseAddress = null)
	{
		getBridgeBaseAddress ??= d;

		return services
			.AddSingleton(new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				NumberHandling = JsonNumberHandling.AllowReadingFromString,
			})
			.AddHttpClient<Helpers.PhilipsHue.IClient, Helpers.PhilipsHue.Concrete.Client>(name: "philipshue-client", (provider, client) =>
			{
				client.BaseAddress = getBridgeBaseAddress(provider);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			.AddHttpClient<Helpers.PhilipsHue.IDiscoveryClient, Helpers.PhilipsHue.Concrete.DiscoveryClient>(name: "philipshue-discovery-client", (provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.PhilipsHue.Config>>().Value;
				client.BaseAddress = config.DiscoveryEndPoint;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddTransient<Helpers.PhilipsHue.IService, Helpers.PhilipsHue.Concrete.Service>();

		static Uri d(IServiceProvider provider)
		{
			var disco = provider.GetRequiredService<Helpers.PhilipsHue.IDiscoveryClient>();
			var ip = disco.GetBridgeIPAddressAsync().GetAwaiter().GetResult();
			return new UriBuilder("http", ip.ToString()).Uri;
		}
	}
}
