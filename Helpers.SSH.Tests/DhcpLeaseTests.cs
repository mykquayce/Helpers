using System;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.SSH.Tests
{
	public class DhcpLeaseTests
	{
		[Theory]
		[InlineData(
			"1616568370 4c:3b:df:97:98:3e 192.168.1.201 XBOX 01:4c:3b:df:97:98:3e",
			"2021-03-24T06:46:10Z", "4c3bdf97983e", "192.168.1.201", "XBOX", "01:4c:3b:df:97:98:3e")]
		[InlineData(
			"1616572635 3c:6a:9d:14:d7:65 192.168.1.217 * *",
			"2021-03-24T07:57:15Z", "3c6a9d14d765", "192.168.1.217", default, default)]
		public void ParseDhcpEntry(
			string s,
			string expectedExpiration, string expectedPhysicalAddress, string expectedIPAddress, string? expectedHostName, string? expectedIdentifier)
		{
			var actual = Helpers.SSH.Services.Concrete.SSHService.GetDhcpLease(s);

			Assert.NotEqual(default, actual.Expiration);
			Assert.Equal(DateTime.Parse(expectedExpiration), actual.Expiration);
			Assert.Equal(PhysicalAddress.Parse(expectedPhysicalAddress), actual.PhysicalAddress);
			Assert.Equal(IPAddress.Parse(expectedIPAddress), actual.IPAddress);
			Assert.Equal(expectedHostName, actual.HostName);
			Assert.Equal(expectedIdentifier, actual.Identifier);
		}
	}
}
