using System;

namespace Helpers.RabbitMQ
{
	public interface IRabbitMQService : IDisposable
	{
		void Acknowledge(ulong deliveryTag);
		(byte[] bytes, ulong deliveryTag) Consume(string queue);
		(T value, ulong deliveryTag) Consume<T>(string queue);
		void Publish(string queue, byte[] bytes);
		void Publish<T>(string queue, T value);
	}
}
