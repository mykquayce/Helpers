using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class SocketClientTests : IClassFixture<Fixtures.SocketClientFixture>
	{
		private readonly EndPoint _endPoint;
		private readonly Clients.ISocketClient _sut;

		public SocketClientTests(Fixtures.SocketClientFixture socketClientFixture)
		{
			var (ipAddress, _) = Helpers.Networking.NetworkHelpers.PingAsync(socketClientFixture.HostName)
				.GetAwaiter().GetResult();

			var port = socketClientFixture.BroadcastPort;

			_endPoint = new IPEndPoint(ipAddress, port);
			_sut = socketClientFixture.SocketClient;
		}

		[Fact]
		public Task Connect() => _sut.ConnectAsync(_endPoint);

		[Theory]
		[InlineData("sendir,1:1,1,40192,3,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task Send(string message)
		{
			await Connect();
			var bytes = Encoding.UTF8.GetBytes(message);
			var response = await _sut.SendAsync(bytes);
			Assert.InRange(response, 1, int.MaxValue);
		}

		[Theory]
		[InlineData("sendir,1:1,1,40192,3,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,1\r")]
		public async Task Receive(string message, string expected)
		{
			await Send(message);

			var bytesResult = await _sut.ReceiveAsync();
			Assert.NotNull(bytesResult);
			Assert.NotEmpty(bytesResult);
			var actual = Encoding.UTF8.GetString(bytesResult);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(
			"sendir,1:1,1,40192,3,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
			"completeir,1:1,1\r")]
		public async Task SendAndReceive(string message, string expected)
		{
			await Connect();
			var actual = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, actual);
		}
	}
}
