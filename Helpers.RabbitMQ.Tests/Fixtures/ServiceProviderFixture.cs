using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.RabbitMQ.Tests.Fixtures;

public sealed class ServiceProviderFixture : ConfigurationFixture, IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public ServiceProviderFixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddRabbitMQ(base.Configuration)
			.BuildServiceProvider();
	}

	public global::RabbitMQ.Client.IChannel Channel => _serviceProvider.GetRequiredService<global::RabbitMQ.Client.IChannel>();
	public global::RabbitMQ.Client.IConnection Connection => _serviceProvider.GetRequiredService<global::RabbitMQ.Client.IConnection>();
	public IService Service => _serviceProvider.GetRequiredService<IService>();
	public Config Config => _serviceProvider.GetRequiredService<IOptions<Config>>().Value;

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
