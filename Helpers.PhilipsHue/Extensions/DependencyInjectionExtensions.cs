using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, IConfiguration configuration)
	{
		Guard.Argument(configuration["username"]!)
			.NotNull().NotEmpty().NotWhiteSpace();

		return services
			.Configure<Helpers.PhilipsHue.Config>(configuration)
			.AddPhilipsHue();
	}

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, string? physicalAddress, string? hostName, string username)
	{
		Guard.Argument(username).NotNull().NotEmpty().NotWhiteSpace();

		var config = new Helpers.PhilipsHue.Config(physicalAddress?.ToString().ToLowerInvariant(), hostName, username);
		return services
			.AddPhilipsHue(config);
	}

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services, Helpers.PhilipsHue.Config config)
	{
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddPhilipsHue();
	}

	public static IServiceCollection AddPhilipsHue(this IServiceCollection services)
	{
		return services
			.AddSingleton(new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				NumberHandling = JsonNumberHandling.AllowReadingFromString,
			})
			.AddHttpClient<Helpers.PhilipsHue.IClient, Helpers.PhilipsHue.Concrete.Client>(name: "philipshue-client", (provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.PhilipsHue.Config>>().Value;
				client.BaseAddress = config.BaseAddress;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			.TryAddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddTransient<Helpers.PhilipsHue.IService, Helpers.PhilipsHue.Concrete.Service>();
	}

	public static IServiceCollection TryAddSingleton<TService>(
		this IServiceCollection collection, TService instance)
		where TService : class
	{
		ServiceCollectionDescriptorExtensions.TryAddSingleton<TService>(collection, instance);
		return collection;
	}
}
