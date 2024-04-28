using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Elgato.Tests;

public sealed class Fixture : IDisposable
{
	private static readonly Uri _baseAddress = new("http://192.168.1.217:9123/");
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddElgato(_baseAddress)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.BuildServiceProvider();

		WhiteLightClient = _serviceProvider.GetRequiredService<IWhiteLightClient>();
		WhiteLightService = _serviceProvider.GetRequiredService<IWhiteLightService>();
	}

	public IWhiteLightClient WhiteLightClient { get; }
	public IWhiteLightService WhiteLightService { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
