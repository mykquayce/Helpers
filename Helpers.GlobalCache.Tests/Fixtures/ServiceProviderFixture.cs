using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Helpers.GlobalCache.Tests.Fixtures;

public class ServiceProviderFixture : IAsyncDisposable
{
	public ServiceProviderFixture()
	{
		IConfiguration configuration = new XUnitClassFixtures.UserSecretsFixture().Configuration;

		ServiceProvider = new ServiceCollection()
			.AddGlobalCache(configuration.GetSection("globalcache"), configuration.GetSection("globalcache:messages"))
			.BuildServiceProvider();
	}

	public IServiceProvider ServiceProvider { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "<Pending>")]
	public ValueTask DisposeAsync() => ((ServiceProvider)ServiceProvider).DisposeAsync();
}
