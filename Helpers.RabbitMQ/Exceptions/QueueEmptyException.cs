namespace Helpers.RabbitMQ.Exceptions;

[Serializable]
public class QueueEmptyException : Exception
{
	public QueueEmptyException(string queueName)
		: base("Queue empty: " + queueName)
	{
		base.Data.Add(nameof(queueName), queueName);
	}
}
