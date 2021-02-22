using System.Net.Sockets;
using System.Text.Json;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class SocketClientConfigTests
	{
		[Theory]
		[InlineData(@"{}", 1_024, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream)]
		[InlineData(@"{""BufferSize"":1023}", 1_023, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream)]
		[InlineData(@"{""BufferSize"":1023,""AddressFamily"":8}", 1_023, AddressFamily.Ecma)]
		[InlineData(@"{""BufferSize"":1023,""AddressFamily"":""Ecma""}", 1_023, AddressFamily.Ecma)]
		[InlineData(@"{""BufferSize"":1023,""AddressFamily"":""InterNetwork"",""ProtocolType"":""Udp""}", 1_023, AddressFamily.InterNetwork, ProtocolType.Udp)]
		[InlineData(@"{""BufferSize"":1023,""AddressFamily"":""InterNetwork"",""ProtocolType"":""Udp"",""SocketType"":""Raw""}", 1_023, AddressFamily.InterNetwork, ProtocolType.Udp, SocketType.Raw)]
		public void ConfigTests(string json, int expectedBufferSize, AddressFamily? expectedAddressFamily = default, ProtocolType? expectedProtocolType = default, SocketType? expectedSocketType = default)
		{
			var config = JsonSerializer.Deserialize<Helpers.Networking.Clients.Concrete.SocketClient.Config>(json);
			Assert.Equal(expectedBufferSize, config.BufferSize);
			if (expectedAddressFamily is not null) Assert.Equal(expectedAddressFamily!.Value, config.AddressFamily);
			if (expectedProtocolType is not null) Assert.Equal(expectedProtocolType!.Value, config.ProtocolType);
			if (expectedSocketType is not null) Assert.Equal(expectedSocketType!.Value, config.SocketType);
		}
	}
}
