using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	[Collection("Non-Parallel Collection")]
	public class TPLinkServiceTests
	{
		private readonly ITPLinkClient _client = new Concrete.TPLinkClient();
		private readonly ITPLinkService _service = new Concrete.TPLinkService();

		[Fact]
		public async Task GetRealtimeDataTests()
		{
			double amps, watts, volts;
			var devices = await _client.DiscoverAsync().ToListAsync();

			Assert.NotEmpty(devices);

			foreach (var (alias, ip, mac) in devices)
			{
				(amps, watts, volts) = await _service.GetRealtimeDataAsync(alias);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);

				(amps, watts, volts) = await _service.GetRealtimeDataAsync(ip);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);

				(amps, watts, volts) = await _service.GetRealtimeDataAsync(mac);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);
			}
		}
	}
}
