using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Helpers.TPLink.Concrete;

public class Service(UdpClient client) : IService
{
	public async IAsyncEnumerable<(string, IPEndPoint, PhysicalAddress)> DiscoverAsync(IPEndPoint endPoint, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new { system = new { get_sysinfo = new { }, }, }
			.Serialize().Encode().Encrypt();
		var responses = client.SendAndReceiveAsyncEnumerable(endPoint, request, cancellationToken);
		await foreach (var (responseEndPoint, responseBytes) in responses)
		{
			var (system, _) = responseBytes.Decrypt().Decode().Deserialize<Models.ResponseObject>();
			var (alias, mac, _, _) = system.get_sysinfo;
			yield return (alias, responseEndPoint, mac);
		}
	}

	public async Task<(double amps, double volts, double watts)> GetRealtimeDataAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default)
	{
		var request = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, }
			.Serialize().Encode().Encrypt();
		var (_, responseBytes) = await client.SendAndReceiveAsync(endPoint, request, cancellationToken);
		var (_, emeter) = responseBytes.Decrypt().Decode()
			.Deserialize<Models.ResponseObject>();
		var (milliamps, millivolts, milliwatts) = emeter.get_realtime;
		return (milliamps / 1_000d, millivolts / 1_000d, milliwatts / 1_000d);
	}

	public async Task<bool> GetStateAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default)
	{
		var info = await GetSystemInfoAsync(endPoint, cancellationToken);
		return info.relay_state > 0;
	}

	public async Task<Models.SystemInfoObject> GetSystemInfoAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default)
	{
		var request = new { system = new { get_sysinfo = new { }, }, }
			.Serialize().Encode().Encrypt();
		var (_, responseBytes) = await client.SendAndReceiveAsync(endPoint, request, cancellationToken);
		var (system, _) = responseBytes.Decrypt().Decode()
			.Deserialize<Models.ResponseObject>();
		return system.get_sysinfo;
	}

	public Task SetStateAsync(IPEndPoint endPoint, bool on, CancellationToken cancellationToken = default)
	{
		var request = new { system = new { set_relay_state = new { state = on ? 1 : 0, }, }, }
			.Serialize().Encode().Encrypt();
		return client.SendAsync(request, endPoint, cancellationToken).AsTask();
	}
}
