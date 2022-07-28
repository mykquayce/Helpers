using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using Xunit;

namespace Helpers.Networking.Models.Tests;

public class DhcpLeaseTests
{
	[Theory]
	[InlineData(1_620_891_591, "000c1e059cad", "192.168.1.121", "iTach059CAD", "*")]
	public void DhcpLease(int expiration, string physicalAddressString, string ipAddressString, string hostName, string identifier)
	{
		var lease = new DhcpLease(
			DateTime.UnixEpoch.AddSeconds(expiration),
			PhysicalAddress.Parse(physicalAddressString),
			IPAddress.Parse(ipAddressString),
			hostName,
			identifier);

		var json = JsonSerializer.Serialize(lease);

		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.NotEqual("{}", json);
		Assert.Matches(@"^{""Expiration"":""\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\dZ"",""PhysicalAddress"":""[\d\w]{12}"",""IPAddress"":""\d+\.\d+\.\d+\.\d+"",""HostName"":"".+?"",""Identifier"":"".+?""}$", json);
	}
}
