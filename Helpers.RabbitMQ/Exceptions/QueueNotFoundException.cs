namespace Helpers.RabbitMQ.Exceptions;

[Serializable]
public class QueueNotFoundException : Exception
{
	public QueueNotFoundException(string queueName)
		: base("Queue not found: " + queueName)
	{
		base.Data.Add(nameof(queueName), queueName);
	}
}
