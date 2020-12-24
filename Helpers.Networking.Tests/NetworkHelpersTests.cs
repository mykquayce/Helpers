using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class NetworkHelpersTests
	{
		[Fact]
		public async Task PingEntireNetwork()
		{
			var results = await NetworkHelpers.PingEntireNetworkAsync().ToListAsync();

			Assert.NotNull(results);
			Assert.NotEmpty(results);

			foreach (var (ip, status) in results)
			{
				Assert.NotNull(ip);
				Assert.NotEqual(default, ip);
				Assert.NotEqual(default, status);
			}
		}

		[Theory]
		[InlineData("192.168.1.50", IPStatus.Success)]
		public async Task Ping(string ipAddressString, IPStatus expected)
		{
			var ipAddress = IPAddress.Parse(ipAddressString);
			var status = await NetworkHelpers.PingAsync(ipAddress);
			Assert.Equal(expected, status);
		}

		[Theory]
		[InlineData("3c6a9d14d765", "192.168.1.217")]
		public void RunArpCommand(string physicalAddressString, string expectedIPAddressString)
		{
			// Act
			var arpResultsCollection = NetworkHelpers.RunArpCommand();

			// Assert
			Assert.NotNull(arpResultsCollection);
			Assert.NotEmpty(arpResultsCollection);

			foreach (var results in arpResultsCollection)
			{
				Assert.NotNull(results);

				Assert.NotNull(results.IPAddress);
				Assert.NotNull(results.Results);
				Assert.NotEmpty(results.Results);

				foreach (var result in results.Results)
				{
					Assert.NotNull(result);
					Assert.NotNull(result.IPAddress);
					Assert.NotEqual(default, result.IPAddress);
					Assert.NotNull(result.PhysicalAddress);
					Assert.NotEqual(default, result.PhysicalAddress);
					Assert.NotEqual(Models.ArpResult.Types.None, result.Type);
				}
			}

			Assert.Equal(
				IPAddress.Parse(expectedIPAddressString),
				arpResultsCollection.GetIPAddressFromPhysicalAddress(PhysicalAddress.Parse(physicalAddressString)));
		}
	}
}
