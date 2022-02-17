using Dawn;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Helpers.RabbitMQ.Concrete;

public class Service : IDisposable, IService
{
	public record Config(string Hostname, ushort Port, string Username, string Password, string VirtualHost = Config._defaultVirtualHost)
		: IOptions<Config>
	{
		private const string _defaultHostname = "localhost";
		private const ushort _defaultPort = 5_672;
		private const string _defaultUsername = ConnectionFactory.DefaultUser;
		private const string _defaultPassword = ConnectionFactory.DefaultPass;
		private const string _defaultVirtualHost = ConnectionFactory.DefaultVHost;

		public Config()
			: this(_defaultHostname, _defaultPort, _defaultUsername, _defaultPassword, _defaultVirtualHost)
		{ }

		public static Config Defaults => new();

		public Config Value => this;
	}

	private readonly IConnectionFactory _connectionFactory;
	private IConnection? _connection;
	private IModel? _channel;
	private readonly ICollection<string> _queues = new List<string>();

	public Service(IOptions<Config> configOptions)
	{
		var config = configOptions.Value;

		_connectionFactory = new ConnectionFactory
		{
			HostName = config.Hostname,
			Port = config.Port,
			UserName = config.Username,
			Password = config.Password,
			VirtualHost = config.VirtualHost,
		};
	}

	private void Connect(params string[] queues)
	{
		if (_connection?.IsOpen != true)
		{
			_connection = _connectionFactory.CreateConnection();
		}

		if (_channel?.IsOpen != true)
		{
			_channel = _connection.CreateModel();
		}

		foreach (var queue in queues.Except(_queues))
		{
			_channel.QueueDeclare(
				queue,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: default);

			_queues.Add(queue);
		}
	}

	public void Enqueue(string queue, byte[] body)
	{
		Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();
		Guard.Argument(() => body).NotNull().NotEmpty();
		Connect(queue);
		_channel!.BasicPublish(exchange: string.Empty, routingKey: queue, body: body);
	}

	public (byte[] body, ulong tag) Dequeue(string queue)
	{
		Guard.Argument(() => queue).NotNull().NotEmpty().NotWhiteSpace();
		Connect(queue);
		BasicGetResult result;
		try
		{
			result = _channel!.BasicGet(queue, autoAck: false);
		}
		catch (OperationInterruptedException exception) when (exception.ShutdownReason?.ReplyCode == 404)
		{
			throw new Exceptions.QueueNotFoundException(queue);
		}

		if (result is null)
		{
			throw new Exceptions.QueueEmptyException(queue);
		}

		return (result.Body.ToArray(), result.DeliveryTag);
	}

	public void Acknowledge(ulong tag) => _channel?.BasicAck(tag, multiple: false);

	#region idisposable implementation
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				foreach (var queue in _queues)
				{
					_channel?.QueueDelete(queue, ifUnused: true, ifEmpty: true);
				}
				_channel?.Dispose();
				_connection?.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion idisposable implementation
}
