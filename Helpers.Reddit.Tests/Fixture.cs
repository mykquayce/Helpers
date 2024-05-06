using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Reddit.Tests;

public sealed class Fixture : IDisposable
{
	private static readonly Uri _baseAddress = new("https://old.reddit.com/", UriKind.Absolute);
	private const string _userAgent = "Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405";

	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var services = new ServiceCollection();

		// handlers
		services
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddRateLimitHandler(replenishmentPeriod: TimeSpan.FromSeconds(1), tokenLimit: 1, tokensPerPeriod: 1)
			.AddUserAgentHandler(_userAgent);

		services.AddReddit(baseAddress: _baseAddress)
			.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
			.AddHttpMessageHandler<UserAgentHandler>()
			.AddHttpMessageHandler<RateLimitHandler>();

		_serviceProvider = services.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
		Service = _serviceProvider.GetRequiredService<IService>();
	}

	public IClient Client { get; }
	public IService Service { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
