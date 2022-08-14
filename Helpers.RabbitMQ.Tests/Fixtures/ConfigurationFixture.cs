using Microsoft.Extensions.Configuration;

namespace Helpers.RabbitMQ.Tests.Fixtures;

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
