using Dawn;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Helpers.TPLink.Concrete;

public class Client : IClient
{
	private readonly UdpClient _udpClient;

	public Client(UdpClient udpClient)
	{
		_udpClient = udpClient;
	}

	public IAsyncEnumerable<Models.RealtimeInfoObject> GetRealtimeDataAsync(IPAddress ip, CancellationToken cancellationToken = default)
	{
		Guard.Argument(ip).NotNull().NotEqual(IPAddress.None);
		var request = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, };
		return SendAndReceiveAsync(ip, request, cancellationToken)
			.Select(o => o.emeter.get_realtime);
	}

	public IAsyncEnumerable<Models.SystemInfoObject> GetSystemInfoAsync(IPAddress ip, CancellationToken cancellationToken = default)
	{
		Guard.Argument(ip).NotNull().NotEqual(IPAddress.None);
		var request = new { system = new { get_sysinfo = new { }, }, };
		return SendAndReceiveAsync(ip, request, cancellationToken)
			.Select(o => o.system.get_sysinfo);
	}

	public ValueTask<int> SetStateAsync(IPAddress ip, bool state, CancellationToken cancellationToken = default)
	{
		// {"system":{"set_relay_state":{"state":1}}}
		var request = new { system = new { set_relay_state = new { state = state ? 1 : 0, }, }, };
		return SendAsync(ip, request, cancellationToken);
	}

	#region private methods
	private ValueTask<int> SendAsync(IPAddress ip, object request, CancellationToken cancellationToken = default)
	{
		var requestBytes = request.Serialize().Encode().Encrypt();
		var endPoint = new IPEndPoint(ip, Constants.Port);
		return _udpClient.SendAsync(requestBytes, endPoint, cancellationToken);
	}

	private async IAsyncEnumerable<Models.ResponseObject> SendAndReceiveAsync(
		IPAddress ip,
		object request,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestBytes = request.Serialize().Encode().Encrypt();
		var endPoint = new IPEndPoint(ip, Constants.Port);
		var results = _udpClient.SendAndReceiveAsync(endPoint, requestBytes, cancellationToken);
		await foreach ((_, byte[] bytes) in results)
		{
			var o = bytes.Decrypt().Decode().Deserialize<Models.ResponseObject>();
			yield return o;
		}
	}
	#endregion private methods
}
