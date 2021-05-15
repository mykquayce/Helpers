using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Helpers.Infrared.Tests.ClientTests
{
	public class Config
	{
		[Theory]
		[InlineData(@"{""Port"":123,""NewLine"":""\n""}")]
		[InlineData(@"{""Port"":1234,""NewLine"":""\r\n""}")]
		public void Configuration(string json)
		{
			// Arrange
			var bytes = Encoding.UTF8.GetBytes(json);
			using var stream = new MemoryStream(bytes);

			var configuration = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			// Act
			var actual = configuration.Get<Clients.Concrete.GlobalCacheClient.Config>();

			// Assert
			Assert.NotNull(actual);
			Assert.NotEqual(0, actual.Port);
			Assert.NotNull(actual.NewLine);
		}

		[Theory]
		[InlineData(@"{""Port"":123,""NewLine"":""\n""}")]
		[InlineData(@"{""Port"":1234,""NewLine"":""\r\n""}")]
		public void ServiceProvider(string json)
		{
			// Arrange
			var bytes = Encoding.UTF8.GetBytes(json);
			using var stream = new MemoryStream(bytes);

			IConfiguration configuration = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			IServiceProvider serviceProvider = new ServiceCollection()
				.Configure<Clients.Concrete.GlobalCacheClient.Config>(configuration)
				.BuildServiceProvider();

			// Act
			var options = serviceProvider.GetService<IOptions<Clients.Concrete.GlobalCacheClient.Config>>();

			// Assert
			Assert.NotNull(options);
			Assert.NotNull(options!.Value);
			Assert.NotEqual(0, options.Value.Port);
			Assert.NotNull(options.Value.NewLine);
		}

		[Theory]
		[InlineData(123, "\n")]
		[InlineData(1234, "\r\n")]
		public void SerializeDeserialize(ushort port, string newLine)
		{
			var before = new Clients.Concrete.GlobalCacheClient.Config(port, newLine);
			var json = JsonSerializer.Serialize(before);
			var after = JsonSerializer.Deserialize<Clients.Concrete.GlobalCacheClient.Config>(json);

			Assert.NotNull(after);
			Assert.Equal(before, after);
			Assert.Equal(port, after!.Port);
			Assert.Equal(newLine, after.NewLine);
		}

		[Theory]
		[InlineData(@"{""Port"":123,""NewLine"":""\n"",""Repeat"":234,""Pause"":345}")]
		[InlineData(@"{""Port"":1234,""NewLine"":""\r\n"",""Repeat"":2345,""Pause"":3456}")]
		public void DeserializeSerialize(string before)
		{
			var o = JsonSerializer.Deserialize<Clients.Concrete.GlobalCacheClient.Config>(before);
			var after = JsonSerializer.Serialize(o);
			Assert.Equal(before, after);
		}
	}
}
