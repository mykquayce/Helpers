using Dawn;
using OpenTracing;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Collections.Generic;
using System.Text.Json;

namespace Helpers.RabbitMQ.Concrete
{
	public class RabbitMQService : IRabbitMQService
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			IgnoreNullValues = false,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		private ITracer? _tracer;
		private readonly IConnection _connection;
		private readonly IModel _model;
		private static readonly ICollection<string> _queueNames = new List<string>();

		public RabbitMQService(
			Models.Settings settings,
			ITracer? tracer = default)
		{
			_tracer = tracer;

			using var scope = _tracer?
				.BuildSpan(nameof(RabbitMQService))
				.WithTag(nameof(Models.Settings.HostName), settings.HostName)
				.WithTag(nameof(Models.Settings.Port), settings.Port)
				.WithTag(nameof(Models.Settings.UserName), settings.UserName)
				.WithTag(nameof(Models.Settings.VirtualHost), settings.VirtualHost)
				.StartActive(finishSpanOnDispose: true);

			Guard.Argument(() => settings).NotNull();

			Guard.Argument(() => settings.HostName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Port).InRange(1, 65_535);
			Guard.Argument(() => settings.UserName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Password).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.VirtualHost).NotNull().NotEmpty().NotWhiteSpace();

			var connectionFactory = new ConnectionFactory
			{
				HostName = settings.HostName,
				Port = settings.Port,
				UserName = settings.UserName,
				Password = settings.Password,
				VirtualHost = settings.VirtualHost,
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
			using var scope = _tracer?
				.BuildSpan(nameof(Acknowledge))
				.WithTag(nameof(deliveryTag), deliveryTag.ToString("D"))
				.StartActive(finishSpanOnDispose: true);

			Guard.Argument(() => deliveryTag).NotDefault();

			_model.BasicAck(deliveryTag, multiple: false);
		}
		#endregion Acknowledge

		#region Consume
		public (byte[] bytes, ulong deliveryTag) Consume(string queue)
		{
			using var scope = _tracer?
				.BuildSpan(nameof(Consume))
				.WithTag(nameof(queue), queue)
				.StartActive(finishSpanOnDispose: true);

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

			if (result == default)
			{
				throw new Exceptions.QueueEmptyException(queue);
			}

			Guard.Argument(() => result).NotNull();
			Guard.Argument(() => result.Body).NotNull().NotEmpty();

			return (result.Body, result.DeliveryTag);
		}

		public (T value, ulong deliveryTag) Consume<T>(string queue)
		{
			var span = _tracer?.ActiveSpan;

			using var scope = span == default
				? _tracer?
					.BuildSpan(nameof(Consume))
					.WithTag(new OpenTracing.Tag.StringTag(nameof(queue)), queue)
					.StartActive(finishSpanOnDispose: true)
				: default;

			var (bytes, deliveryTag) = Consume(queue);

			var value = JsonSerializer.Deserialize<T>(bytes, _jsonSerializerOptions);

			(span ?? scope?.Span)?.SetTag(nameof(value), value?.ToString());

			return (value, deliveryTag);
		}
		#endregion Consume

		#region Publish
		public void Publish(string queue, byte[] bytes)
		{
			using var scope = _tracer?
				.BuildSpan(nameof(Consume))
				.WithTag(new OpenTracing.Tag.StringTag(nameof(queue)), queue)
				.WithTag(new OpenTracing.Tag.IntTag(nameof(bytes)), bytes.Length)
				.StartActive(finishSpanOnDispose: true);

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
				JsonSerializer.SerializeToUtf8Bytes<T>(value, _jsonSerializerOptions));
		} 
		#endregion Publish
	}
}
