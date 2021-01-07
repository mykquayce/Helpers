using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class TcpClientTests
	{
		[Theory]
		[InlineData("192.34.234.30", 43, "domain google.com\r\n")]
		[InlineData("riswhois.ripe.net", 43, "-F -K -i 32934\r\n")]
		public async Task SendAndReceive(string hostname, int port, string message)
		{
			using var sut = new Helpers.Networking.Clients.Concrete.TcpClient(hostname, port);

			var response = await sut.SendAndReceiveAsync(message);

			Assert.NotNull(response);
			Assert.NotEmpty(response);
		}
	}
}
