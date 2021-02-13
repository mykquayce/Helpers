using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection("Non-Parallel Collection")]
	public class UdpClientTests : IClassFixture<Fixtures.UdpClientFixture>
	{
		private readonly Clients.IUdpClient _sut;

		public UdpClientTests(Fixtures.UdpClientFixture fixture)
		{
			_sut = fixture.UdpClient;
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", @"^http:\/\/192\.168\.1\.\d+$")]
		public async Task Discover(string expectedUuid, string expectedConfigUrlPattern)
		{
			var beacons = await _sut.DiscoverAsync().ToListAsync();

			Assert.Single(beacons);
			Assert.DoesNotContain(default, beacons);

			var beacon = beacons.Single();

			Assert.Equal(expectedUuid, beacon.Uuid);
			Assert.Matches(expectedConfigUrlPattern, beacon.ConfigUrl);
		}
	}
}
