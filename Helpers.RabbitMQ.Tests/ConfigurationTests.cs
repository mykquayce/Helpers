using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Helpers.RabbitMQ.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ConfigurationTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly IConfiguration _configuration;
	private readonly string _queueName;

	public ConfigurationTests(Fixtures.ConfigurationFixture fixture)
	{
		_configuration = fixture.Configuration;
		_queueName = fixture.QueueName;
	}

	[Theory]
	[InlineData("one", "two", "three")]
	public void Configuration(params string[] messages)
	{
		using var serviceProvider = new ServiceCollection()
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

		using var serviceProvider = new ServiceCollection()
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

		using var serviceProvider = new ServiceCollection()
			.AddRabbitMQ(config.Hostname, config.Port, config.Username, config.Password, config.VirtualHost, config.SslEnabled, config.QueueNames)
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

	[Fact]
	public void FileSupportTests()
	{
		Config config;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["Hostname"] = "qwfpqwfqqwfpq",
					["Port"] = "12345",
					["Username_file"] = Path.Combine(".", "Data", "Username"),
					["Password_file"] = Path.Combine(".", "Data", "Password"),
					["VirtualHost"] = "dhnpgjpgjlpgjl",
					["SslEnabled"] = "true",
					["QueueNames:0"] = "airsontaersnot",
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			using var serviceProvider = new ServiceCollection()
				.AddRabbitMQ(configuration)
				.BuildServiceProvider();

			config = serviceProvider.GetRequiredService<IOptions<Config>>().Value;
		}

		Assert.NotNull(config);

		Assert.NotNull(config.Hostname);
		Assert.NotEqual(default, config.Port);
		Assert.NotNull(config.Username);
		Assert.NotNull(config.Password);
		Assert.NotNull(config.VirtualHost);
		Assert.NotNull(config.QueueNames);
		Assert.NotEmpty(config.QueueNames);
		Assert.DoesNotContain(default, config.QueueNames);

		Assert.NotEqual(RabbitMQ.Config.DefaultHostname, config.Hostname);
		Assert.NotEqual(RabbitMQ.Config.DefaultPort, config.Port);
		Assert.NotEqual(RabbitMQ.Config.DefaultUsername, config.Username);
		Assert.NotEqual(RabbitMQ.Config.DefaultPassword, config.Password);
		Assert.NotEqual(RabbitMQ.Config.DefaultVirtualHost, config.VirtualHost);
		Assert.All(config.QueueNames, s => Assert.NotEqual(RabbitMQ.Config.DefaultQueueName, s));
	}
}
