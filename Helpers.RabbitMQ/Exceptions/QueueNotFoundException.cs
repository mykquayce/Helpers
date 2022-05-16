namespace Helpers.RabbitMQ.Exceptions;

[Serializable]
public class QueueNotFoundOrEmptyException : Exception
{
	public QueueNotFoundOrEmptyException(string queueName)
		: base("Queue not found or empty: " + queueName)
	{
		base.Data.Add(nameof(queueName), queueName);
	}
}
