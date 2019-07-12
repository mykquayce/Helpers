using Dawn;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Helpers.RabbitMQ.Concrete
{
	public class RabbitMQService : IRabbitMQService
	{
		private readonly IConnection _connection;
		private readonly IModel _model;
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true, };
		private static readonly ICollection<string> _queueNames = new List<string>();

		public RabbitMQService(
			string hostName = "localhost",
			int port = 5672,
			string userName = ConnectionFactory.DefaultUser,
			string password = ConnectionFactory.DefaultPass,
			string virtualHost = ConnectionFactory.DefaultVHost)
		{
			Guard.Argument(() => hostName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => userName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => password).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => virtualHost).NotNull().NotEmpty().NotWhiteSpace();

			var connectionFactory = new ConnectionFactory
			{
				HostName = hostName,
				Port = port,
				UserName = userName,
				Password = password,
				VirtualHost = virtualHost,
			};

			_connection = connectionFactory.CreateConnection();

			_model = _connection.CreateModel();

			_model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
		}

		public void Dispose()
		{
			_model?.Dispose();
			_connection?.Dispose();
		}

		#region Acknowledge
		public void Acknowledge(ulong deliveryTag)
		{
			Guard.Argument(() => deliveryTag).NotDefault();

			_model.BasicAck(deliveryTag, multiple: false);
		}
		#endregion Acknowledge

		#region Consume
		public (byte[] bytes, ulong deliveryTag) Consume(string queue)
		{
			Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();

			BasicGetResult result;

			try
			{
				result = _model.BasicGet(queue, autoAck: false);
			}
			catch (OperationInterruptedException exception) when (exception.ShutdownReason?.ReplyCode == 404)
			{
				throw new Exceptions.QueueNotFoundException(queue);
			}

			Guard.Argument(() => result).NotNull();
			Guard.Argument(() => result.Body).NotNull().NotEmpty();

			return (result.Body, result.DeliveryTag);
		}

		public (T value, ulong deliveryTag) Consume<T>(string queue)
		{
			var (bytes, deliveryTag) = Consume(queue);

			var value = JsonSerializer.Parse<T>(bytes, _jsonSerializerOptions);

			return (value, deliveryTag);
		}
		#endregion Consume

		#region Publish
		public void Publish(string queue, byte[] bytes)
		{
			Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => bytes).NotNull().NotEmpty();

			if (!_queueNames.Contains(queue))
			{
				_model.QueueDeclare(
					queue: queue,
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: default);

				_queueNames.Add(queue);
			}

			_model.BasicPublish(
				exchange: string.Empty,
				routingKey: queue,
				mandatory: false,
				basicProperties: default,
				body: bytes);
		}

		public void Publish<T>(string queue, T value)
		{
			Publish(
				queue,
				JsonSerializer.ToUtf8Bytes<T>(value, _jsonSerializerOptions));
		} 
		#endregion Publish
	}
}
