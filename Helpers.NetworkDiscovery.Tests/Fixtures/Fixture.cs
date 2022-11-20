using Microsoft.Extensions.DependencyInjection;

namespace Helpers.NetworkDiscovery.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var secrets = new XUnitClassFixtures.UserSecretsFixture();
		var configuration = secrets.Configuration;

		_serviceProvider = new ServiceCollection()
			.Configure<Config>(configuration.GetSection("networkdiscovery"))
			.Configure<Identity.Config>(configuration.GetSection("identity"))
			.AddNetworkDiscovery()
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
	}

	public IClient Client { get; }

	public void Dispose() => ((ServiceProvider)_serviceProvider).Dispose();
}
