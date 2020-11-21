using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	public class SocketClientTests : IClassFixture<Fixtures.SocketClientFixture>
	{
		private readonly Clients.ISocketClient _sut;

		public SocketClientTests(Fixtures.SocketClientFixture fixture)
		{
			_sut = fixture.SocketClient;
		}

		[Theory]
		[InlineData("192.168.1.114:4998")]
		public ValueTask Connect(string ipEndPointString)
		{
			var ipEndPoint = IPEndPoint.Parse(ipEndPointString);
			return _sut.ConnectAsync(ipEndPoint);
		}

		[Theory]
		[InlineData("192.168.1.114:4998", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task Send(string ipEndPointString, string message)
		{
			await Connect(ipEndPointString);

			var tasks = Enumerable.Repeat(
				_sut.SendAsync(message).AsTask(),
				count: 5);

			var ints = await Task.WhenAll(tasks);

			Assert.All(ints, i => Assert.Equal(i, ints[0]));
		}

		[Theory]
		[InlineData("192.168.1.114:4998", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,4\r")]
		public async Task Receive(string ipEndPointString, string message, string expected)
		{
			await Send(ipEndPointString, message);
			var result = await _sut.ReceiveAsync();
			Assert.Equal((byte)'c', result[0]);
			var actual = Encoding.UTF8.GetString(result);
			Assert.Equal(expected, actual);
		}
	}
}
