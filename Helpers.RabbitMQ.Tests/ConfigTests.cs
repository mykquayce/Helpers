using Microsoft.Extensions.Configuration;
using Xunit;

namespace Helpers.RabbitMQ.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData("test-host", 1, "test-username", "test-password", "test-virtualhost", true, "test-queue")]
	[InlineData("test-host", 1, "test-username", "test-password", "test-virtualhost", true, "test-queue1", "test-queue2")]
	public void Test1(string hostname, ushort port, string username, string password, string virtualHost, bool sslEnabled, params string[] queueNames)
	{
		IConfiguration configuration;
		{
			var initialData = new Dictionary<string, string?>
			{
				[nameof(hostname)] = hostname,
				[nameof(port)] = port.ToString("D"),
				[nameof(username)] = username,
				[nameof(password)] = password,
				[nameof(virtualHost)] = virtualHost,
				[nameof(sslEnabled)] = sslEnabled.ToString(),
			};

			for (var a = 0; a < queueNames.Length; a++)
			{
				initialData.Add("queueNames:" + a, queueNames[a]);
			}

			configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();
		}

		var config = Helpers.RabbitMQ.Config.Defaults;
		configuration.Bind(config);

		Assert.Equal(hostname, config.Hostname);
		Assert.Equal(port, config.Port);
		Assert.Equal(username, config.Username);
		Assert.Equal(password, config.Password);
		Assert.Equal(virtualHost, config.VirtualHost);
		Assert.Equal(sslEnabled, config.SslEnabled);
		Assert.Equal(queueNames, config.QueueNames);
	}
}
