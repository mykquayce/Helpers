using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection("Non-Parallel Collection")]
	public class DiscoveryClientTests : IClassFixture<Fixtures.DiscoveryClientFixture>
	{
		private readonly Clients.IDiscoveryClient _sut;

		public DiscoveryClientTests(Fixtures.DiscoveryClientFixture fixture)
		{
			_sut = fixture.DiscoveryClient;
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
