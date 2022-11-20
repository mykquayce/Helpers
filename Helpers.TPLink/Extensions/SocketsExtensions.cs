using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace System.Net.Sockets;

public static class SocketsExtensions
{
	public static async Task BroadcastAsync(this UdpClient udpClient,
		byte[] bytes,
		ushort port,
		CancellationToken cancellationToken = default)
	{
		foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
		{
			if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
			if (nic.OperationalStatus != OperationalStatus.Up) continue;

			var properties = nic.GetIPProperties();

			foreach (var unicast in properties.UnicastAddresses)
			{
				var broadcast = unicast.GetBroadcastAddress();
				var endPoint = new IPEndPoint(broadcast, port);
				await udpClient.SendAsync(bytes, endPoint, cancellationToken);
			}
		}
	}

	public static async IAsyncEnumerable<UdpReceiveResult> SendAndReceiveAsync(this UdpClient udpClient,
		IPEndPoint endPoint,
		byte[] bytes,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await udpClient.SendAsync(bytes, endPoint, cancellationToken);

		while (!cancellationToken.IsCancellationRequested)
		{
			UdpReceiveResult result;
			try
			{
				result = await udpClient.ReceiveAsync(cancellationToken);
			}
			catch (OperationCanceledException) { continue; }
			yield return result;
			if (!cancellationToken.CanBeCanceled) { break; }
		}
	}

	public static void Deconstruct(this UdpReceiveResult result, out IPEndPoint endPoint, out byte[] bytes)
	{
		endPoint = result.RemoteEndPoint;
		bytes = result.Buffer;
	}
}
