using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.RabbitMQ.Tests;

public class DependencyInjectionTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
{
	private readonly IConfiguration _configuration;
	private readonly string _queueName;

	public DependencyInjectionTests(Helpers.XUnitClassFixtures.UserSecretsFixture fixture)
	{
		_configuration = fixture.Configuration.GetSection("RabbitMQ");
		_queueName = _configuration["QueueName"] ?? throw new Exception();
	}

	[Theory]
	[InlineData("one", "two", "three")]
	public void Configuration(params string[] messages)
	{
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddRabbitMQ(_configuration)
			.BuildServiceProvider();

		ServiceProviderTests(serviceProvider, messages);
	}

	[Theory]
	[InlineData("one", "two", "three")]
	public void Config(params string[] messages)
	{
		var config = Helpers.RabbitMQ.Config.Defaults;
		_configuration.Bind(config);

		IServiceProvider serviceProvider = new ServiceCollection()
			.AddRabbitMQ(config)
			.BuildServiceProvider();

		ServiceProviderTests(serviceProvider, messages);
	}

	[Theory]
	[InlineData("one", "two", "three")]
	public void Value(params string[] messages)
	{
		var config = Helpers.RabbitMQ.Config.Defaults;
		_configuration.Bind(config);

		IServiceProvider serviceProvider = new ServiceCollection()
			.AddRabbitMQ(config.Hostname, config.Port, config.Username, config.Password, config.VirtualHost, config.SslEnabled, config.QueueName)
			.BuildServiceProvider();

		ServiceProviderTests(serviceProvider, messages);
	}

	private void ServiceProviderTests(IServiceProvider serviceProvider, params string[] messages)
	{
		var service = serviceProvider.GetRequiredService<Helpers.RabbitMQ.IService>();

		try
		{
			foreach (var message in messages)
			{
				service.Enqueue(_queueName, message);

				var (actual, tag) = service.Dequeue<string>(_queueName);

				service.Acknowledge(tag);

				Assert.Equal(message, actual);
			}
		}
		finally
		{
			service.DeleteQueue(_queueName);
		}
	}
}
