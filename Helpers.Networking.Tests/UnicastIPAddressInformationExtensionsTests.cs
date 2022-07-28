using Moq;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Tests;

public class UnicastIPAddressInformationExtensionsTests
{
	[Theory]
	[InlineData("192.168.1.100", "255.255.255.0", "192.168.1.255")]
	public void GetBroadcastAddressTests(string ip, string mask, string expected)
	{
		// Arrange
		var unicastMock = new Mock<UnicastIPAddressInformation>();
		unicastMock.Setup(i => i.Address).Returns(IPAddress.Parse(ip));
		unicastMock.Setup(i => i.IPv4Mask).Returns(IPAddress.Parse(mask));
		var unicast = unicastMock.Object;

		// Act
		var actual = unicast.GetBroadcastAddress();

		// Assert
		Assert.NotNull(actual);
		Assert.Equal(expected, actual.ToString());
	}
}
