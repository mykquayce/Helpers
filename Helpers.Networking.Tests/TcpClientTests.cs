using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class TcpClientTests
	{
		[Theory]
		[InlineData("192.34.234.30", 43, "\n", "domain google.com\n")]
		[InlineData("riswhois.ripe.net", 43, "\n", "-F -K -i 14618\n")]
		[InlineData("riswhois.ripe.net", 43, "\n", "-F -K -i 32934\n")]
		[InlineData("iTach059CAD", 4_998, "\r", "sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r")]
		public async Task SendAndReceive(string hostname, ushort port, string newLine, string message)
		{
			var sut = new Helpers.Networking.Clients.Concrete.TcpClient(hostname, port, newLine);

			var responses = await sut.SendAndReceiveAsync(message).ToListAsync();

			Assert.NotNull(responses);
			Assert.NotEmpty(responses);
			Assert.DoesNotContain("% No entries found", responses, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
