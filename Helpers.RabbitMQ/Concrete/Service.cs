using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Helpers.RabbitMQ.Concrete;
public class Service : IService
{
	private readonly IModel _channel;

	public Service(IModel channel)
	{
		ArgumentNullException.ThrowIfNull(channel);
		_channel = channel;
	}

	public void Enqueue(string queue, byte[] body)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		ArgumentNullException.ThrowIfNull(body);

		_channel.BasicPublish(exchange: string.Empty, routingKey: queue, mandatory: true, body: body);
	}

	public void CreateQueue(string queue)
	{
		_channel.QueueDeclare(
			queue: queue,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: default);
	}

	public (byte[] body, ulong tag) Dequeue(string queue, bool autoAcknowledge = false)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		BasicGetResult result;
		try
		{
			result = _channel.BasicGet(queue, autoAcknowledge);
		}
		catch (OperationInterruptedException exception)
			when (exception.ShutdownReason.ReplyCode == 404)
		{
			throw new Exceptions.QueueNotFoundException(queue);
		}

		if (result is null)
		{
			throw new Exceptions.QueueEmptyException(queue);
		}

		return (result.Body.ToArray(), result.DeliveryTag);
	}

	public void Acknowledge(ulong tag) => _channel.BasicAck(tag, multiple: false);

	public void PurgeQueue(string queue)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		while (_channel.BasicGet(queue, autoAck: true) is not null) { }
	}

	public void DeleteQueue(string queue)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		try { _channel.QueueDelete(queue, ifEmpty: false); }
		catch (AlreadyClosedException) { }
	}
}
