using System;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class TcpClientTests
	{
		[Theory]
		[InlineData("192.34.234.30", 43, "domain google.com\n")]
		[InlineData("riswhois.ripe.net", 43, "-F -K -i 32934\n")]
		public async Task SendAndReceive(string hostname, ushort port, string message)
		{
			var sut = new Helpers.Networking.Clients.Concrete.TcpClient(hostname, port);

			var response = await sut.SendAndReceiveAsync(message);

			Assert.NotNull(response);
			Assert.NotEmpty(response);
			Assert.DoesNotContain("% No entries found", response, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
