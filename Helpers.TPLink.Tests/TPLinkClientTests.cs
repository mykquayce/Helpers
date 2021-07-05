using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
	public class TPLinkClientTests
	{
		private readonly ITPLinkClient _sut = new Concrete.TPLinkClient();

		[Fact]
		public async Task DiscoverTests()
		{
			var localIPAddresses = Concrete.TPLinkClient.LocalIPAddresses.ToList();

			var devices = await _sut.DiscoverAsync().ToListAsync();
			Assert.NotEmpty(devices);

			foreach (var device in devices)
			{
				var (alias, ip, mac) = device;
				Assert.NotNull(alias);
				Assert.NotNull(ip);
				Assert.DoesNotContain(ip, localIPAddresses);
				Assert.NotNull(mac);
			}
		}

		[Fact]
		public async Task GetRealtimeDataTests()
		{
			var devices = await _sut.DiscoverAsync().ToListAsync();

			foreach (var (_, ip, _) in devices)
			{
				var (amps, watts, volts) = await _sut.GetRealtimeDataAsync(ip);

				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);
			}
		}
	}
}
