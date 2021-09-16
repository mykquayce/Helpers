using Dawn;
using Helpers.Tracing;
using OpenTracing;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text.Json;

namespace Helpers.RabbitMQ.Concrete
{
	public class RabbitMQService : IRabbitMQService
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		private IConnection? _connection;
		private IModel? _model;
		private readonly ITracer? _tracer;
		private readonly IConnectionFactory _connectionFactory;

		public RabbitMQService(
			Models.RabbitMQSettings settings,
			ITracer? tracer = default)
		{
			_tracer = tracer;

			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(settings);

			try
			{
				Guard.Argument(() => settings).NotNull();

				Guard.Argument(() => settings.HostName).NotNull().NotEmpty().NotWhiteSpace();
				Guard.Argument(() => settings.Port).InRange(1, 65_535);
				Guard.Argument(() => settings.UserName).NotNull().NotEmpty().NotWhiteSpace();
				Guard.Argument(() => settings.Password, secure: true).NotNull().NotEmpty().NotWhiteSpace();
				Guard.Argument(() => settings.VirtualHost).NotNull().NotEmpty().NotWhiteSpace();
			}
			catch (Exception ex)
			{
				scope?.Span.Log(ex);
				throw;
			}

			_connectionFactory = new ConnectionFactory
			{
				HostName = settings.HostName,
				Port = settings.Port,
				UserName = settings.UserName,
				Password = settings.Password,
				VirtualHost = settings.VirtualHost,
			};
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_model?.Dispose();
					_connection?.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

		#region Acknowledge
		public void Acknowledge(ulong deliveryTag)
		{
			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(nameof(deliveryTag), deliveryTag);

			Guard.Argument(() => deliveryTag).NotDefault();

			Connect();

			_model!.BasicAck(deliveryTag, multiple: false);
		}
		#endregion Acknowledge

		#region Consume
		public (byte[] bytes, ulong deliveryTag) Consume(string queue)
		{
			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(nameof(queue), queue);

			Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();

			Connect(queue);

			BasicGetResult result;

			try
			{
				try
				{
					result = _model!.BasicGet(queue, autoAck: false);
				}
				catch (OperationInterruptedException exception) when (exception.ShutdownReason?.ReplyCode == 404)
				{
					throw new Exceptions.QueueNotFoundException(queue);
				}

				if (result is null)
				{
					throw new Exceptions.QueueEmptyException(queue);
				}
			}
			catch (Exception exception)
			{
				exception.Data.Add(nameof(queue), queue);

				scope?.Span.Log(exception);

				throw;
			}

			Guard.Argument(() => result).NotNull().Wrap(r => r.Body)
				.NotDefault();

			scope?.Span.Log(result);

			return (result.Body.Span.ToArray(), result.DeliveryTag);
		}

		public (T value, ulong deliveryTag) Consume<T>(string queue)
		{
			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(nameof(queue), queue);

			var (bytes, deliveryTag) = Consume(queue);

			scope?.Span.Log(
				nameof(bytes), bytes,
				nameof(deliveryTag), deliveryTag);

			var value = JsonSerializer.Deserialize<T>(bytes, _jsonSerializerOptions);

			scope?.Span.Log(value);

			return (value!, deliveryTag);
		}
		#endregion Consume

		#region Publish
		public void Publish(string queue, byte[] bytes)
		{
			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(
				nameof(queue), queue,
				nameof(bytes), bytes);

			Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => bytes).NotNull().NotEmpty();

			Connect(queue);

			_model!.BasicPublish(
				exchange: string.Empty,
				routingKey: queue,
				basicProperties: default,
				body: bytes);
		}

		public void Publish<T>(string queue, T value)
		{
			Publish(
				queue,
				JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions));
		}
		#endregion Publish

		public void Connect(string? queue = default)
		{
			using var scope = _tracer?.StartSpan();
			scope?.Span.Log(nameof(queue), queue);

			if (_connection is null || !_connection.IsOpen)
			{
				try
				{
					_connection = _connectionFactory.CreateConnection();
				}
				catch (BrokerUnreachableException ex)
				{
					ex.Data.Add(nameof(queue), queue);
					ex.Data.Add(nameof(IConnectionFactory.Uri), _connectionFactory.Uri?.OriginalString);
					ex.Data.Add(nameof(IConnectionFactory.VirtualHost), _connectionFactory.VirtualHost);
					ex.Data.Add(nameof(IConnectionFactory.UserName), _connectionFactory.UserName);

					scope?.Span.Log(ex);

					throw;
				}
			}

			if (_model?.IsOpen == true)
			{
				return;
			}

			_model = _connection.CreateModel();

			_model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

			if (queue == (default))
			{
				return;
			}

			_model.QueueDeclare(
				queue: queue,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: default);
		}
	}
}
