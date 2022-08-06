using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Helpers.RabbitMQ;

public record Config(string Hostname, ushort Port, ushort RestApiPort, string RestApiScheme, string Username, string Password, string VirtualHost, bool SslEnabled, params string[] QueueNames)
	: IOptions<Config>
{
	public const string DefaultHostname = "localhost";
	public const ushort DefaultPort = 5_672;
	public const ushort DefaultRestApiPort = 15_672;
	public const string DefaultRestApiScheme = "http";
	public const string DefaultUsername = ConnectionFactory.DefaultUser;
	public const string DefaultPassword = ConnectionFactory.DefaultPass;
	public const string DefaultVirtualHost = ConnectionFactory.DefaultVHost;
	public const bool DefaultSslEnabled = false;
	public const string DefaultQueueName = "queue";

	public Config()
		: this(DefaultHostname, DefaultPort, DefaultRestApiPort, DefaultRestApiScheme, DefaultUsername, DefaultPassword, DefaultVirtualHost, DefaultSslEnabled)
	{ }

	public static Config Defaults => new();

	public Config Value => this;
}
