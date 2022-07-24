using System.Net;

namespace Helpers.Networking.Tests;

public class PingClientTests : IClassFixture<Fixtures.PingClientFixture>
{
	private readonly Clients.IPingClient _sut;

	public PingClientTests(Fixtures.PingClientFixture fixture)
	{
		_sut = fixture.PingClient;
	}

	[Theory]
	[InlineData("lse.packetlosstest.com", "45.56.112.112")]
	[InlineData("lau.packetlosstest.com", "172.105.174.146")]
	public async Task ResolveHostNameTests(string hostName, string expected)
	{
		var ip = await GetHostAddressAsync(hostName);
		Assert.Equal(expected, ip.ToString());
	}

	private async static Task<IPAddress> GetHostAddressAsync(string hostName)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		var ips = await Dns.GetHostAddressesAsync(hostName, System.Net.Sockets.AddressFamily.InterNetwork, cts.Token);
		return ips.First();
	}

	[Theory]
	[InlineData(10_000, "lse.packetlosstest.com")]
	public async Task PacketLoss(int timeToRun, string hostName)
	{
		var ip = await GetHostAddressAsync(hostName);

		var results = await _sut.PacketLossTestAsync(ip, timeToRun);

		Assert.NotEqual(0, results.Count);
		Assert.Equal(0, results.PacketLossPercentage);
	}
}
