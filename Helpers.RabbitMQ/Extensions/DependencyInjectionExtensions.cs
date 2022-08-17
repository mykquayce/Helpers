using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	#region configurerabbitmq
	public static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.FileConfigure<Helpers.RabbitMQ.Config>(configuration);
	}

	public static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services,
		string hostname, ushort port, string username, string password, string virtualHost, bool sslEnabled, params string[] queueNames)
	{
		var config = new Helpers.RabbitMQ.Config(hostname, port, username, password, virtualHost, sslEnabled, queueNames);
		return services
			.ConfigureRabbitMQ(config);
	}

	public static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services, Helpers.RabbitMQ.Config config)
	{
		var options = Options.Options.Create(config);

		return services
			.ConfigureRabbitMQ(options);
	}

	public static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services, IOptions<Helpers.RabbitMQ.Config> options)
	{
		return services
			.AddSingleton(options);
	}
	#endregion configurerabbitmq

	#region addrabbitmq
	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.ConfigureRabbitMQ(configuration)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services,
		string hostname, ushort port, string username, string password, string virtualHost, bool sslEnabled, params string[] queueNames)
	{
		return services
			.ConfigureRabbitMQ(hostname, port, username, password, virtualHost, sslEnabled, queueNames)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Helpers.RabbitMQ.Config config)
	{
		return services
			.ConfigureRabbitMQ(config)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IOptions<Helpers.RabbitMQ.Config> options)
	{
		return services
			.ConfigureRabbitMQ(options)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
	{
		return services
			.AddSingleton<RabbitMQ.Client.ConnectionFactory>(provider =>
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
				var factory = provider.GetRequiredService<RabbitMQ.Client.ConnectionFactory>();

				var count = 10;
				Exception? exception = null;

				while (count-- >= 0)
				{
					try
					{
						return factory.CreateConnection();
					}
					catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
					{
						exception = ex;
						Thread.Sleep(millisecondsTimeout: 3_000);
					}
				}

				throw exception!;
			})
			.AddScoped<RabbitMQ.Client.IModel>(provider =>
			{
				var config = provider.GetConfig<Helpers.RabbitMQ.Config>();
				var connection = provider.GetRequiredService<RabbitMQ.Client.IConnection>();

				var model = connection.CreateModel();

				foreach (var queueName in config.QueueNames)
				{
					model.QueueDeclare(
						queue: queueName,
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: default);
				}

				return model;
			})
			.AddScoped<Helpers.RabbitMQ.IService, Helpers.RabbitMQ.Concrete.Service>();
	}
	#endregion addrabbitmq
}
