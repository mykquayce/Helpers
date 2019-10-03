using System;
using Xunit;

namespace Helpers.RabbitMQ.Tests
{
	public sealed class RabbitMQServiceTests : IDisposable
	{
		private readonly IRabbitMQService _service;

		public RabbitMQServiceTests()
		{
			var rabbitMqSettings = new Concrete.RabbitMQSettings
			{
				HostName = "localhost",
				Port = 5_672,
				UserName = "guest",
				Password = "guest",
				VirtualHost = "/"
			};

			_service = new Concrete.RabbitMQService(rabbitMqSettings);
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

		[Theory]
		[InlineData("test", "https://old.reddit.com/r/random/.rss")]
		public void RabbitMQServiceTests_ConsumePublishConsumeAckhowledge(string queue, string message)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(message);

			try
			{
				_service.Consume(queue);
				Assert.True(false);
			}
			catch (Exceptions.QueueEmptyException)
			{ }
			catch (Exceptions.QueueNotFoundException)
			{ }

			_service.Publish(queue, bytes);

			var (actualBytes, deliveryTag) = _service.Consume(queue);

			var actual = System.Text.Encoding.UTF8.GetString(actualBytes);

			Assert.Equal(message, actual);
			Assert.NotEqual(default, deliveryTag);

			_service.Acknowledge(deliveryTag);
		}

		private class Class { public string? String { get; set; } }
	}
}
