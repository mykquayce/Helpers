using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class ClientTests : IClassFixture<Fixtures.ClientFixture>
	{
		private readonly IClient _sut;

		public ClientTests(Fixtures.ClientFixture fixture)
		{
			_sut = fixture.Client;
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
