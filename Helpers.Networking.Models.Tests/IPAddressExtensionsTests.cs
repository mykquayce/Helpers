using System.Net;
using Xunit;

namespace Helpers.Networking.Models.Tests;

public class IPAddressExtensionsTests
{
	[Theory]
	[InlineData("2c42:2b60:5155:bc90:d52b:7eb9:d005:d856", "58829603181888090389241792566722418774")]
	public void IPv6AddressToUint128(string ipString, string expected)
	{
		var ip = IPAddress.Parse(ipString);
		var integer = ip.GetUInt128();

		Assert.Equal(UInt128.Parse(expected), integer);
	}
}
