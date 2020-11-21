using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	public class UdpClientTests : IClassFixture<Fixtures.UdpClientFixture>
	{
		private readonly Clients.IUdpClient _sut;

		public UdpClientTests(Fixtures.UdpClientFixture fixture)
		{
			_sut = fixture.UdpClient;
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "http://192.168.1.114")]
		public async Task Discover(string expectedUuid, string expectedConfigUrl)
		{
			var beacon = await _sut.DiscoverAsync().FirstAsync();

			Assert.Equal(expectedUuid, beacon.Uuid);
			Assert.Equal(expectedConfigUrl, beacon.ConfigUrl);
		}
	}
}
