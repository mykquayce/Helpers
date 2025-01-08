using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Helpers.RabbitMQ.Concrete;

public class Service(IChannel channel) : IService
{
	public ValueTask EnqueueAsync(string queue, byte[] body, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		ArgumentNullException.ThrowIfNull(body);

		return channel.BasicPublishAsync(exchange: string.Empty, routingKey: queue, mandatory: true, body: body, cancellationToken);
	}

	public Task<QueueDeclareOk> CreateQueueAsync(string queue, CancellationToken cancellationToken = default)
	{
		return channel.QueueDeclareAsync(
			queue: queue,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: default,
			noWait: false,
			cancellationToken);
	}

	public async ValueTask<(byte[] body, ulong tag)> DequeueAsync(string queue, bool autoAcknowledge = false, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		BasicGetResult? result;
		try
		{
			result = await channel.BasicGetAsync(queue, autoAcknowledge, cancellationToken);
		}
		catch (OperationInterruptedException exception)
			when (exception.ShutdownReason?.ReplyCode == 404)
		{
			throw new Exceptions.QueueNotFoundException(queue);
		}

		if (result is null)
		{
			throw new Exceptions.QueueEmptyException(queue);
		}

		return (result.Body.ToArray(), result.DeliveryTag);
	}

	public ValueTask AcknowledgeAsync(ulong tag, CancellationToken cancellationToken = default)
		=> channel.BasicAckAsync(tag, multiple: false, cancellationToken);

	public Task<uint> PurgeQueueAsync(string queue, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		return channel.QueuePurgeAsync(queue, cancellationToken);
	}

	public Task<uint> DeleteQueueAsync(string queue, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(queue);
		return channel.QueueDeleteAsync(queue, ifUnused: true, ifEmpty: false, cancellationToken);
	}
}
