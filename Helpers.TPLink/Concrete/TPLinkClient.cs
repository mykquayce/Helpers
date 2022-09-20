using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

namespace Helpers.TPLink.Concrete;

public class TPLinkClient : ITPLinkClient
{
	private readonly ushort _port;
	private readonly static object _discoveryObject = new { system = new { get_sysinfo = new { }, }, };
	private readonly static object _getRealtimeDataObject = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, };

	public TPLinkClient(IOptions<Config> options)
	{
		_port = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Wrap(c => c.Port).Value;
	}

	public async IAsyncEnumerable<Models.Device> DiscoverAsync()
	{
		var addresses = (
			from unicast in Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses()
			let broadcast = unicast.GetBroadcastAddress()
			select broadcast
			).ToArray();

		var responses = SendAndReceiveAsync(_discoveryObject, addresses);

		await foreach (var (ip, response) in responses)
		{
			var sysInfo = (Models.SystemInfo)response!.system.get_sysinfo;
			yield return new(sysInfo.Alias, ip, sysInfo.PhysicalAddress);
		}
	}

	public async Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip)
	{
		var (_, response) = await SendAndReceiveAsync(_getRealtimeDataObject, ip).FirstAsync();
		return (Models.RealtimeData)response!.emeter!.get_realtime;
	}

	public async Task<Models.SystemInfo> GetSystemInfoAsync(IPAddress ip)
	{
		(_, var response) = await SendAndReceiveAsync(_discoveryObject, ip).FirstAsync();
		return (Models.SystemInfo)response.system.get_sysinfo;
	}

	public async Task<bool> GetStateAsync(IPAddress ip)
	{
		(_, var response) = await SendAndReceiveAsync(_discoveryObject, ip).FirstAsync();
		return response.system.get_sysinfo.relay_state == 1;
	}

	public Task SetStateAsync(IPAddress ip, bool state)
	{
		// {"system":{"set_relay_state":{"state":1}}}
		var o = new { system = new { set_relay_state = new { state = state ? 1 : 0, }, }, };
		return SendAndReceiveAsync(o, ip).FirstAsync().AsTask();
	}

	private async IAsyncEnumerable<(IPAddress, Models.Generated.ResponseObject)> SendAndReceiveAsync(object request, params IPAddress[] addresses)
	{
		Guard.Argument(request).NotNull();
		Guard.Argument(addresses).NotEmpty().DoesNotContainNull();

		var message = request.Serialize().Encode().Encrypt();

		using var client = new UdpClient(_port, AddressFamily.InterNetwork);

		await Task.WhenAll(from ip in addresses
						   let ep = new IPEndPoint(ip, _port)
						   let task = client.SendAsync(message, message.Length, ep)
						   select task);

		ICollection<UdpReceiveResult> responses = new List<UdpReceiveResult>();
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);

			while (!cts.IsCancellationRequested)
			{
				try
				{
					var response = await client.ReceiveAsync(cts.Token);
					if (message.SequenceEqual(response.Buffer)) continue;
					responses.Add(response);
				}
				catch (OperationCanceledException) { continue; }
			}
		}

		foreach (var response in responses)
		{
			yield return (
				response.RemoteEndPoint.Address,
				response.Buffer.Decrypt().Decode().Deserialize<Models.Generated.ResponseObject>());
		}
	}
}
