﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Helpers.RabbitMQ;

public record Config(string Hostname, ushort Port, string Username, string Password, string VirtualHost = Config.DefaultVirtualHost, bool SslEnabled = Config.DefaultSslEnabled, string QueueName = Config.DefaultQueueName)
	: IOptions<Config>
{
	public const string DefaultHostname = "localhost";
	public const ushort DefaultPort = 5_672;
	public const string DefaultUsername = ConnectionFactory.DefaultUser;
	public const string DefaultPassword = ConnectionFactory.DefaultPass;
	public const string DefaultVirtualHost = ConnectionFactory.DefaultVHost;
	public const bool DefaultSslEnabled = false;
	public const string DefaultQueueName = "queue";

	public Config()
		: this(DefaultHostname, DefaultPort, DefaultUsername, DefaultPassword, DefaultVirtualHost, DefaultSslEnabled, DefaultQueueName)
	{ }

	public static Config Defaults => new();

	public Config Value => this;
}