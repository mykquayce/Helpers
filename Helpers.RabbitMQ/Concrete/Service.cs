using Dawn;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Helpers.RabbitMQ.Concrete;
public class Service : IService
{
	private readonly IModel _channel;

	public Service(IModel channel)
	{
		_channel = Guard.Argument(channel).NotNull().Value;
	}

	public void Enqueue(string queue, byte[] body)
	{
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		Guard.Argument(body).NotNull().NotEmpty();

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

	public (byte[] body, ulong tag) Dequeue(string queue)
	{
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		BasicGetResult result;
		try
		{
			result = _channel.BasicGet(queue, autoAck: false);
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
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		while (_channel.BasicGet(queue, autoAck: true) is not null) { }
	}

	public void DeleteQueue(string queue)
	{
		Guard.Argument(queue).NotNull().NotEmpty().NotWhiteSpace();
		try { _channel.QueueDelete(queue, ifEmpty: false); }
		catch (AlreadyClosedException) { }
	}
}
