using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
	{
		var resolvedConfiguration = new ConfigurationBuilder()
			.AddConfiguration(configuration)
			.ResolveFileReferences()
			.Build();

		return services
			.Configure<Helpers.RabbitMQ.Config>(resolvedConfiguration)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services,
		string hostname, ushort port, string username, string password, string virtualHost, bool sslEnabled, params string[] queueNames)
	{
		var config = new Helpers.RabbitMQ.Config(hostname, port, username, password, virtualHost, sslEnabled, queueNames);
		return services
			.AddRabbitMQ(config);
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Helpers.RabbitMQ.Config config)
	{
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
	{
		return services
			.AddSingleton<RabbitMQ.Client.IConnectionFactory, RabbitMQ.Client.ConnectionFactory>(provider =>
			{
				var config = provider.GetConfig<Helpers.RabbitMQ.Config>();

				return new()
				{
					HostName = config.Hostname,
					Port = config.Port,

					UserName = config.Username,
					Password = config.Password,

					VirtualHost = config.VirtualHost,
					Ssl = new RabbitMQ.Client.SslOption
					{
						Enabled = config.SslEnabled,
						ServerName = config.Hostname,
						AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.None,
					},
				};
			})
			.AddScoped<RabbitMQ.Client.IConnection>(provider =>
			{
				var factory = provider.GetRequiredService<RabbitMQ.Client.IConnectionFactory>();

				var count = 10;
				Exception? exception = null;

				while (count-- >= 0)
				{
					try
					{
						return factory.CreateConnectionAsync().GetAwaiter().GetResult();
					}
					catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
					{
						exception = ex;
						Thread.Sleep(millisecondsTimeout: 3_000);
					}
				}

				throw exception!;
			})
			.AddScoped<RabbitMQ.Client.IChannel>(provider =>
			{
				var config = provider.GetConfig<Helpers.RabbitMQ.Config>();
				var connection = provider.GetRequiredService<RabbitMQ.Client.IConnection>();

				var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

				foreach (var queueName in config.QueueNames)
				{
					channel.QueueDeclareAsync(
						queue: queueName,
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: default).GetAwaiter().GetResult();
				}

				return channel;
			})
			.AddScoped<Helpers.RabbitMQ.IService, Helpers.RabbitMQ.Concrete.Service>();
	}
}
