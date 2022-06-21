using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddRabbitMQ(this IServiceCollection services,
		string hostname, ushort port, string username, string password, string virtualHost, bool sslEnabled, string queueName)
	{
		var config = new Helpers.RabbitMQ.Config(hostname, port, username, password, virtualHost, sslEnabled, queueName);
		return services.AddRabbitMQ(config);
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IOptions<Helpers.RabbitMQ.Config> configOptions)
	{
		return services
			.AddSingleton(configOptions)
			.AddRabbitMQ();
	}

	public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.RabbitMQ.Config>(configuration)
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
					Ssl = new SslOption
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
					catch (BrokerUnreachableException ex)
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

				model.QueueDeclare(
					queue: config.QueueName,
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: default);

				return model;
			})
			.AddScoped<Helpers.RabbitMQ.IService, Helpers.RabbitMQ.Concrete.Service>();
	}
}
