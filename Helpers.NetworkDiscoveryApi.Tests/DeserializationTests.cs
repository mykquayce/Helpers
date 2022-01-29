using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

public class DeserializationTests
{
	[Theory]
	[InlineData(@"[{""expiration"":""2021-12-08T13:49:04Z"",""physicalAddress"":""b827eb4476aa"",""ipAddress"":""192.168.1.158"",""hostName"":""raspberrypi"",""identifier"":""01:b8:27:eb:44:76:aa""},{""expiration"":""2021-12-08T08:18:55Z"",""physicalAddress"":""1800db0e980d"",""ipAddress"":""192.168.1.170"",""hostName"":""Aria2"",""identifier"":null},{""expiration"":""2021-12-08T07:53:37Z"",""physicalAddress"":""aec11783aa8f"",""ipAddress"":""192.168.1.106"",""hostName"":""Galaxy-Tab-S7"",""identifier"":""01:ae:c1:17:83:aa:8f""},{""expiration"":""2021-12-08T06:34:55Z"",""physicalAddress"":""c6c7817326e2"",""ipAddress"":""192.168.1.205"",""hostName"":""OnePlus-8T"",""identifier"":""01:c6:c7:81:73:26:e2""},{""expiration"":""2021-12-08T13:31:44Z"",""physicalAddress"":""1c1adf84902e"",""ipAddress"":""192.168.1.121"",""hostName"":""XBOX"",""identifier"":""01:1c:1a:df:84:90:2e""},{""expiration"":""2021-12-08T11:38:30Z"",""physicalAddress"":""003192e1a474"",""ipAddress"":""192.168.1.248"",""hostName"":null,""identifier"":null},{""expiration"":""2021-12-08T15:36:10Z"",""physicalAddress"":""003192e1a68b"",""ipAddress"":""192.168.1.219"",""hostName"":""KP115"",""identifier"":null},{""expiration"":""2021-12-08T12:33:30Z"",""physicalAddress"":""f02f74d209a5"",""ipAddress"":""192.168.1.237"",""hostName"":""malik10"",""identifier"":""01:f0:2f:74:d2:09:a5""},{""expiration"":""2021-12-08T14:32:23Z"",""physicalAddress"":""a438ccdecbe2"",""ipAddress"":""192.168.1.111"",""hostName"":null,""identifier"":null},{""expiration"":""2021-12-08T14:13:58Z"",""physicalAddress"":""ecb5fa18e324"",""ipAddress"":""192.168.1.157"",""hostName"":""Philips-hue"",""identifier"":null},{""expiration"":""2021-12-08T10:47:58Z"",""physicalAddress"":""00c2c6cb3531"",""ipAddress"":""192.168.1.133"",""hostName"":""johnson"",""identifier"":""01:00:c2:c6:cb:35:31""},{""expiration"":""2021-12-08T15:44:33Z"",""physicalAddress"":""7ea7b0328b3e"",""ipAddress"":""192.168.1.214"",""hostName"":""flichub"",""identifier"":""ff:b0:32:8b:3e:00:01:00:01:c7:92:bc:90:da:fc:ce:74:4e:01""},{""expiration"":""2021-12-08T11:07:47Z"",""physicalAddress"":""28ee52eb0aa4"",""ipAddress"":""192.168.1.143"",""hostName"":null,""identifier"":null},{""expiration"":""2021-12-08T15:21:34Z"",""physicalAddress"":""3c6a9d14d765"",""ipAddress"":""192.168.1.217"",""hostName"":null,""identifier"":null}]")]
	public async Task ManyTests(string json)
	{
		var bytes = System.Text.Encoding.UTF8.GetBytes(json);
		await using var stream = new MemoryStream(bytes);
		stream.Position = 0;
		var leases = await JsonSerializer.DeserializeAsyncEnumerable<Models.DhcpResponseObject>(stream)
			.ToListAsync();

		Assert.NotNull(leases);
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);

		using var enumerator = leases.GetEnumerator();

		while (enumerator.MoveNext())
		{
			var dhcpDetails = enumerator.Current;

			Assert.NotNull(dhcpDetails);

			var (expiration, mac, ip, hostName, identifier) = dhcpDetails;

			Assert.NotEqual(default, expiration);
			Assert.NotEqual(default, mac);
			Assert.NotEqual(default, ip);
			Assert.NotEqual(string.Empty, hostName);
			Assert.NotEqual(string.Empty, identifier);
		}
	}

	[Theory]
	[InlineData(
		@"{""expiration"":""2021-12-08T13:49:04Z"",""physicalAddress"":""b827eb4476aa"",""ipAddress"":""192.168.1.158"",""hostName"":""raspberrypi"",""identifier"":""01:b8:27:eb:44:76:aa""}",
		"2021-12-08T13:49:04Z",
		"b827eb4476aa",
		"192.168.1.158",
		"raspberrypi",
		"01:b8:27:eb:44:76:aa")]
	public void SingleTests(
		string json,
		string expectedExpirationString, string expectedPhysicalAddressString, string expectedIPAddressString, string expectedHostName, string expectedIdentifier)
	{
		var actual = JsonSerializer.Deserialize<Models.DhcpResponseObject>(json);
		Assert.NotNull(actual);
		Assert.Equal(DateTime.Parse(expectedExpirationString), actual!.expiration);
		Assert.Equal(PhysicalAddress.Parse(expectedPhysicalAddressString), actual.physicalAddress);
		Assert.Equal(IPAddress.Parse(expectedIPAddressString), actual.ipAddress);
		Assert.Equal(expectedHostName, actual.hostName);
		Assert.Equal(expectedIdentifier, actual.identifier);
	}
}
