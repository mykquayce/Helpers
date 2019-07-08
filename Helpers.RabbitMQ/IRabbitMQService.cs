using System;

namespace Helpers.RabbitMQ
{
	public interface IRabbitMQService : IDisposable
	{
		void Acknowledge(ulong deliveryTag);
		(T value, ulong deliveryTag) Consume<T>(string queue);
		void Publish<T>(string queue, T value);
	}
}