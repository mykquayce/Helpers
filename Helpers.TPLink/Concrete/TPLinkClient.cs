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
		var endPoints = (
			from unicast in Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses()
			let broadcast = unicast.GetBroadcastAddress()
			select new IPEndPoint(broadcast, _port)
			).ToArray();

		var responses = SendAndReceiveAsync(_discoveryObject, endPoints);

		await foreach (var (ip, response) in responses)
		{
			var sysInfo = (Models.SystemInfo)response!.system.get_sysinfo;
			yield return new(sysInfo.Alias, ip, sysInfo.PhysicalAddress);
		}
	}

	public async Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip)
	{
		var endPoint = new IPEndPoint(ip, _port);
		var (_, response) = await SendAndReceiveAsync(_getRealtimeDataObject, endPoint).FirstAsync();
		return (Models.RealtimeData)response!.emeter!.get_realtime;
	}

	public async Task<Models.SystemInfo> GetSystemInfoAsync(IPAddress ip)
	{
		var endPoint = new IPEndPoint(ip, _port);
		(_, var response) = await SendAndReceiveAsync(_discoveryObject, endPoint).FirstAsync();
		return (Models.SystemInfo)response.system.get_sysinfo;
	}

	public async Task<bool> GetStateAsync(IPAddress ip)
	{
		var endPoint = new IPEndPoint(ip, _port);
		(_, var response) = await SendAndReceiveAsync(_discoveryObject, endPoint).FirstAsync();
		return response.system.get_sysinfo.relay_state == 1;
	}

	public Task SetStateAsync(IPAddress ip, bool state)
	{
		var endPoint = new IPEndPoint(ip, _port);
		// {"system":{"set_relay_state":{"state":1}}}
		var o = new { system = new { set_relay_state = new { state = state ? 1 : 0, }, }, };
		return SendAndReceiveAsync(o, endPoint).FirstAsync().AsTask();
	}

	private async static IAsyncEnumerable<(IPAddress, Models.Generated.ResponseObject)> SendAndReceiveAsync(object request, params IPEndPoint[] endPoints)
	{
		Guard.Argument(request).NotNull();
		Guard.Argument(endPoints).NotEmpty().DoesNotContainNull();

		ICollection<UdpReceiveResult> responses = new List<UdpReceiveResult>();
		{
			var message = request.Serialize().Encode().Encrypt();
			var messageLength = message.Length;

			var ports = endPoints.Select(ep => ep.Port).Distinct();

			foreach (var port in ports)
			{
				using var client = new UdpClient(port);

				var tasks = from ep in endPoints
							let task = client.SendAsync(message, messageLength, ep)
							select task;

				await Task.WhenAll(tasks);

				using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);
				while (!cts.IsCancellationRequested)
				{
					var task = await Task.WhenAny(
						client.ReceiveAsync(),
						Task.Delay(millisecondsDelay: 100, cts.Token));

					if (task is not Task<UdpReceiveResult> myTask) continue;
					var response = await myTask;
					if (message.SequenceEqual(response.Buffer))
					{
						continue;
					}
					responses.Add(response);
				}
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
