using System.Threading.Tasks;
using Xunit;

namespace Helpers.Infrared.Tests.ClientTests
{
	public class GlobalCacheClient
	{
		private readonly Clients.IClient _sut;

		public GlobalCacheClient()
		{
			_sut = new Clients.Concrete.GlobalCacheClient();
		}

		[Theory]
		[InlineData(
			"iTach059CAD",
			"sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r")]
		public async Task Send(string host, string message)
		{
			// mute
			await _sut.SendAsync(host, message);

			// pause
			await Task.Delay(millisecondsDelay: 500);

			// unmute
			await _sut.SendAsync(host, message);
		}
	}
}
