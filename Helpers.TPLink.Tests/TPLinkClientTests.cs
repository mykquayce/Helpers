using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class TPLinkClientTests : IClassFixture<Fixtures.TPLinkClientFixture>
	{
		private readonly ITPLinkClient _sut;

		public TPLinkClientTests(Fixtures.TPLinkClientFixture fixture)
		{
			_sut = fixture.TPLinkClient;
		}

		[Fact]
		public async Task Discover()
		{
			var localIPAddresses = Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses().Select(ip => ip.Address);

			var devices = await _sut.DiscoverAsync().ToListAsync();
			Assert.NotEmpty(devices);

			foreach (var (alias, ip, mac) in devices)
			{
				Assert.NotNull(alias);
				Assert.NotNull(ip);
				Assert.DoesNotContain(ip, localIPAddresses);
				Assert.NotNull(mac);
			}
		}

		[Theory]
		[InlineData("amp")]
		public async Task GetRealtimeDataAsync(string alias)
		{
			var (amps, watts, volts) = await _sut.GetRealtimeDataAsync(alias);

			Assert.InRange(amps, .0001, double.MaxValue);
			Assert.InRange(watts, .0001, double.MaxValue);
			Assert.InRange(volts, .0001, double.MaxValue);
		}
	}
}
