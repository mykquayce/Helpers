using System;

namespace Helpers.RabbitMQ.Exceptions
{
	public class QueueNotFoundException : Exception
	{
		public QueueNotFoundException(string queueName)
			: base("Queue not found: " + queueName)
		{
			base.Data.Add(nameof(queueName), queueName);
		}
	}
}
