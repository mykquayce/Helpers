using System;
using Xunit;

namespace Helpers.RabbitMQ.Tests
{
	public class RabbitMQServiceTests : IDisposable
	{
		private readonly IRabbitMQService _service;

		public RabbitMQServiceTests()
		{
			_service = new Concrete.RabbitMQService();
		}

		public void Dispose()
		{
			_service?.Dispose();
		}

		[Theory]
		[InlineData("test", "message")]
		public void RabbitMQServiceTests_PublishConsumeAcknowledge(string queue, string message)
		{
			var before = new Class { String = message, };

			_service.Publish(queue, before);

			var (actual, deliveryTag) = _service.Consume<Class>(queue);

			Assert.NotNull(actual);
			Assert.Equal(message, actual.String);
			Assert.NotEqual(default, deliveryTag);

			_service.Acknowledge(deliveryTag);
		}

		private class Class { public string String { get; set; } }
	}
}
