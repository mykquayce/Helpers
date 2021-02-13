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
		public void RunArpCommand(string physicalAddressString, string ipAddressString)
		{
			// Arrange
			var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
			var ipAddress = IPAddress.Parse(ipAddressString);

			// Act
			var arpResultsDictionary = NetworkHelpers.RunArpCommand();

			// Assert
			Assert.NotNull(arpResultsDictionary);
			Assert.NotEmpty(arpResultsDictionary);

			foreach (var (ip, results) in arpResultsDictionary)
			{
				Assert.NotNull(results);

				Assert.NotNull(ip);
				Assert.NotNull(results);
				Assert.NotEmpty(results);

				foreach (var result in results)
				{
					Assert.NotNull(result);
					Assert.NotNull(result.IPAddress);
					Assert.NotEqual(default, result.IPAddress);
					Assert.NotNull(result.PhysicalAddress);
					Assert.NotEqual(default, result.PhysicalAddress);
					Assert.NotEqual(Models.ArpResult.Types.None, result.Type);
				}
			}

			Assert.Contains(physicalAddress, arpResultsDictionary.SelectMany(kvp => kvp.Value).Select(r => r.PhysicalAddress));
			Assert.Contains(ipAddress, arpResultsDictionary.SelectMany(kvp => kvp.Value).Select(r => r.IPAddress));
		}

		[Theory]
		[InlineData("3c6a9d14d765")]
		public void IPAddressFromPhysicalAddress(string physicalAddressString)
		{
			var physicalAddress = PhysicalAddress.Parse(physicalAddressString);

			var ipAddress = NetworkHelpers.IPAddressFromPhysicalAddress(physicalAddress);

			Assert.NotNull(ipAddress);
			Assert.NotEqual(default, ipAddress);
		}

		[Theory]
		[InlineData("192.168.1.217")]
		public void PhysicalAddressFromIPAddress(string ipAddressString)
		{
			var ipAddress = IPAddress.Parse(ipAddressString);

			var physicalAddress = NetworkHelpers.PhysicalAddressFromIPAddress(ipAddress);

			Assert.NotNull(physicalAddress);
			Assert.NotEqual(default, physicalAddress);
		}
	}
}
