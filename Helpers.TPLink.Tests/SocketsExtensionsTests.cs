using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Helpers.TPLink.Tests;

public sealed class SocketsExtensionsTests : IDisposable
{
	private readonly UdpClient _sut = new();

	[Theory]
	[InlineData("192.168.1.255:9999")]
	public async Task DiscoveryTests(string endPointString)
	{
		var endPoint = IPEndPoint.Parse(endPointString);
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
		var responses = await DiscoveryAsync(endPoint, cts.Token).ToArrayAsync(cts.Token);
		Assert.NotEmpty(responses);
		Assert.DoesNotContain(default, responses);
	}

	[Theory]
	[InlineData("192.168.1.255:9999")]
	public async Task GetRealtimeDataTests(string endPointString)
	{
		var endPoint = IPEndPoint.Parse(endPointString);
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);
		var data = await GetRealtimeDataAsync(endPoint, cts.Token).ToArrayAsync(cts.Token);

		Assert.NotEmpty(data);
		Assert.DoesNotContain(default, data);

		foreach (var datum in data)
		{
			var (amps, volts, watts) = datum;
			Assert.InRange(amps, .001, 1_000);
			Assert.InRange(volts, 220, 260);
			Assert.InRange(watts, .1, 50);
		}
	}

	private IAsyncEnumerable<IPEndPoint> DiscoveryAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default)
	{
		var o = new { system = new { get_sysinfo = new { }, }, };
		var bytes = o.Serialize().Encode().Encrypt();
		return _sut.SendAndReceiveAsyncEnumerable(endPoint, bytes, cancellationToken)
			.Select(r => r.RemoteEndPoint);
	}

	private async IAsyncEnumerable<(double, double, double)> GetRealtimeDataAsync(IPEndPoint endPoint, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var o = new { emeter = new { get_realtime = new { }, }, };
		var bytes = o.Serialize().Encode().Encrypt();
		await foreach (var ep in DiscoveryAsync(endPoint, cancellationToken))
		{
			var result = await _sut.SendAndReceiveAsync(ep, bytes, cancellationToken);
			var (_, emeter) = result.Buffer.Decrypt().Decode().Deserialize<Models.ResponseObject>();
			var (milliamps, millivolts, milliwatts) = emeter.get_realtime;
			yield return (milliamps / 1_000d, millivolts / 1_000d, milliwatts / 1_000d);
		}
	}

	public void Dispose() => _sut.Dispose();
}
