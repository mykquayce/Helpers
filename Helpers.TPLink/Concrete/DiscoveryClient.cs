using Dawn;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Helpers.TPLink.Concrete;

public class DiscoveryClient : IDiscoveryClient
{
	private readonly UdpClient _udpClient;

	public DiscoveryClient(UdpClient udpClient)
	{
		_udpClient = Guard.Argument(udpClient).NotNull().Value;
	}

	public async IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync([EnumeratorCancellation] CancellationToken cancellationToken)
	{
		Guard.Argument(cancellationToken).NotDefault().Require(ct => ct.CanBeCanceled);

		var request = new { system = new { get_sysinfo = new { }, }, };
		var requestBytes = request.Serialize().Encode().Encrypt();

		// broadcast the request on all available ethernet networks
		await _udpClient.BroadcastAsync(requestBytes, Constants.Port, cancellationToken);

		// receive until the cancellation token runs out
		while (!cancellationToken.IsCancellationRequested)
		{
			IPEndPoint responseEndPoint;
			byte[] responseBytes;
			try
			{
				(responseEndPoint, responseBytes) = await _udpClient.ReceiveAsync(cancellationToken);
			}
			catch (OperationCanceledException) { continue; }

			if (requestBytes.SequenceEqual(responseBytes)) { continue; }
			var o = responseBytes.Decrypt().Decode().Deserialize<Models.ResponseObject>();
			(var alias, var mac, _, _) = o.system.get_sysinfo;
			yield return (alias, responseEndPoint.Address, mac);
		}
	}
}
