namespace Helpers.RabbitMQ.Exceptions;

[Serializable]
public class QueueNotFoundOrEmptyException : Exception
{
	public QueueNotFoundOrEmptyException(string queueName)
		: this(queueName, default)
	{ }

	public QueueNotFoundOrEmptyException(string queueName, Exception? innerException)
		: base("Queue not found or empty: " + queueName, innerException)
	{
		base.Data.Add(nameof(queueName), queueName);
	}
}
