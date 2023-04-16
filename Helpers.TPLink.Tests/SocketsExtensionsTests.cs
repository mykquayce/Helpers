using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Helpers.TPLink.Tests;

public sealed class SocketsExtensionsTests : IDisposable
{
	private readonly UdpClient _sut = new();

	[Theory]
	[InlineData("192.168.1.255:9999", 2)]
	public async Task Discover_GetRealtimeData(string endPointString, int expectedCount)
	{
		IReadOnlyCollection<IPEndPoint> devices = await DiscoveryTests(endPointString).ToArrayAsync();
		Assert.Equal(expectedCount, devices.Count);

		foreach (var endPoint in devices)
		{
			var (amps, volts, watts) = await GetRealtimeDataTests(endPoint.ToString());
			Assert.InRange(amps, .001, 1_000);
			Assert.InRange(volts, 220, 260);
			Assert.InRange(watts, .1, 50);
		}
	}

	[Theory]
	[InlineData("192.168.1.255:9999")]
	public async IAsyncEnumerable<IPEndPoint> DiscoveryTests(string endPointString)
	{
		var endPoint = IPEndPoint.Parse(endPointString);
		var request = new { system = new { get_sysinfo = new { }, }, }
			.Serialize().Encode().Encrypt();
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
		var responses = await _sut.SendAndReceiveAsyncEnumerable(endPoint, request, cts.Token)
			.ToArrayAsync(cts.Token);
		Assert.NotEmpty(responses);
		Assert.All(responses, r => Assert.NotEqual(default, r));
		foreach (var response in responses) { yield return response.RemoteEndPoint; }
	}

	[Theory]
	[InlineData("192.168.1.219:9999")]
	[InlineData("192.168.1.248:9999")]
	public async Task<(double, double, double)> GetRealtimeDataTests(string endPointString)
	{
		var endPoint = IPEndPoint.Parse(endPointString);
		var request = new { emeter = new { get_realtime = new { }, }, }
			.Serialize().Encode().Encrypt();
		var response = await _sut.SendAndReceiveAsync(endPoint, request);
		(_, var emeter) = response.Buffer
			.Decrypt().Decode().Deserialize<Models.ResponseObject>();
		Assert.NotEqual(default, emeter);
		Assert.NotEqual(default, emeter.get_realtime);
		var (milliamps, millivolts, milliwatts) = emeter.get_realtime;
		return (milliamps / 1_000d, millivolts / 1_000d, milliwatts / 1_000d);
	}

	public void Dispose() => _sut.Dispose();
}
