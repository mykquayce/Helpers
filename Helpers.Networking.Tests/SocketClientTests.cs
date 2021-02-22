using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
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
