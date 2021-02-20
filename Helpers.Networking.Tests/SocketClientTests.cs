using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	[Collection("Non-Parallel Collection")]
	public class SocketClientTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
	{
		private readonly EndPoint _endPoint;
		private readonly Clients.ISocketClient _sut;

		public SocketClientTests(Helpers.XUnitClassFixtures.UserSecretsFixture userSecretsFixture)
		{
			var physicalAddressString = userSecretsFixture["Networking:GlobalCache:PhysicalAddress"];
			var portString = userSecretsFixture["Networking:GlobalCache:Port"];

			var physicalAddress = PhysicalAddress.Parse(physicalAddressString);

			var ipAddress = NetworkHelpers.IPAddressFromPhysicalAddress(physicalAddress);
			var port = ushort.Parse(portString);

			_endPoint = new IPEndPoint(ipAddress, port);

			var config = new Helpers.Networking.Clients.Concrete.SocketClient.Config();
			_sut = new Helpers.Networking.Clients.Concrete.SocketClient(config);
		}

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

		[Fact]
		public ValueTask Connect() => _sut.ConnectAsync(_endPoint);

		[Theory]
		[InlineData(5, 50, "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task Send(int count, int millisecondsDelay, string message)
		{
			var responses = new List<int>();

			await Connect();

			while (--count > 0)
			{
				var response = await _sut.SendAsync(message);
				responses.Add(response);
				await Task.Delay(millisecondsDelay);
			}

			Assert.NotEmpty(responses);
			Assert.All(responses, @int => Assert.InRange(@int, 1, int.MaxValue));
			Assert.Equal(responses.Min(), responses.Max());
		}

		[Theory]
		[InlineData(5, 50, "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,4\r")]
		public async Task Receive(int count, int millisecondsDelay, string message, string expected)
		{
			await Send(count, millisecondsDelay, message);

			var result = await _sut.ReceiveAsync();
			Assert.Equal((byte)'c', result[0]);
			var actual = Encoding.UTF8.GetString(result);
			Assert.Equal(expected, actual);
		}
	}
}
