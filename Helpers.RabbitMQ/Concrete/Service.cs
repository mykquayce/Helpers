﻿using Dawn;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Helpers.RabbitMQ.Concrete;

public class Service : IDisposable, IService
{
	public record Config(string Hostname, ushort Port, string Username, string Password, string VirtualHost = Config.DefaultVirtualHost, bool SslEnabled = false)
		: IOptions<Config>
	{
		public const string DefaultHostname = "localhost";
		public const ushort DefaultPort = 5_672;
		public const string DefaultUsername = ConnectionFactory.DefaultUser;
		public const string DefaultPassword = ConnectionFactory.DefaultPass;
		public const string DefaultVirtualHost = ConnectionFactory.DefaultVHost;
		public const bool DefaultSslEnabled = false;

		public Config()
			: this(DefaultHostname, DefaultPort, DefaultUsername, DefaultPassword, DefaultVirtualHost, DefaultSslEnabled)
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
		var config = Guard.Argument(configOptions).NotNull().Wrap(o => o.Value)
			.NotNull().Value;

		_connectionFactory = new ConnectionFactory
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
	}

	private void Connect(params string[] queues)
	{
		Guard.Argument(queues).NotEmpty().DoesNotContainNull().DoesNotContain(string.Empty);

		if (_connection?.IsOpen != true)
		{
			var count = 10;
			while (_connection is null
				&& count-- >= 0)
			{
				try { _connection = _connectionFactory.CreateConnection(); }
				catch (BrokerUnreachableException)
				{
					if (count <= 0) throw;
					Thread.Sleep(millisecondsTimeout: 3_000);
				}
			}
		}

		if (_channel?.IsOpen != true)
		{
			_channel = _connection!.CreateModel();
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
		catch (OperationInterruptedException exception)
		{
			throw new Exceptions.QueueNotFoundOrEmptyException(queue, exception);
		}

		if (result is null)
		{
			throw new Exceptions.QueueNotFoundOrEmptyException(queue);
		}

		return (result.Body.ToArray(), result.DeliveryTag);
	}

	public void Acknowledge(ulong tag) => _channel?.BasicAck(tag, multiple: false);

	public void PurgeQueue(string queue)
	{
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		while (_channel?.BasicGet(queue, autoAck: true) is not null) { }
	}

	public void PurgeQueues()
	{
		foreach (var queue in _queues) PurgeQueue(queue);
	}

	public void DeleteQueue(string queue)
	{
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		try { _channel?.QueueDelete(queue, ifEmpty: false); }
		catch (AlreadyClosedException) { }
	}

	public void DeleteQueues()
	{
		foreach (var queue in _queues) DeleteQueue(queue);
	}

	#region idisposable implementation
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
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
