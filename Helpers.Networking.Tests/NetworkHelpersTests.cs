using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Tests;

public class NetworkHelpersTests
{

	[Theory]
	[InlineData("192.168.1.50", IPStatus.Success)]
	public async Task Ping_IPAddress(string ipAddressString, IPStatus expected)
	{
		var ipAddress = IPAddress.Parse(ipAddressString);
		var status = await NetworkHelpers.PingAsync(ipAddress);
		Assert.Equal(expected, status);
	}

	[Theory]
	[InlineData("iTach059CAD", @"^192\.168\.1\.\d+$", IPStatus.Success)]
	public async Task Ping_HostName(string hostName, string expectedIPAddressPattern, IPStatus expectedStatus)
	{
		var (actualIPAddress, actualStatus) = await NetworkHelpers.PingAsync(hostName);
		Assert.NotNull(actualIPAddress);
		Assert.Matches(expectedIPAddressPattern, actualIPAddress.ToString());
		Assert.Equal(expectedStatus, actualStatus);
	}
}
