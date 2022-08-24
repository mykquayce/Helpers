﻿using Microsoft.Extensions.Configuration;
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

	public global::RabbitMQ.Client.IModel Model => _serviceProvider.GetRequiredService<global::RabbitMQ.Client.IModel>();
	public global::RabbitMQ.Client.IConnection Connection => _serviceProvider.GetRequiredService<global::RabbitMQ.Client.IConnection>();
	public IService Service => _serviceProvider.GetRequiredService<IService>();
	public IClient Client => _serviceProvider.GetRequiredService<IClient>();
	public Config Config => _serviceProvider.GetRequiredService<IOptions<Config>>().Value;

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}

public class ConfigurationFixture
{
	public ConfigurationFixture()
	{
		var secrets = new XUnitClassFixtures.UserSecretsFixture();
		Configuration = secrets.Configuration.GetSection("rabbitmq");
	}

	public IConfiguration Configuration { get; }
	public string QueueName => Configuration["QueueNames:0"] ?? throw new Exception();
}