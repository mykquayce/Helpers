using System.Runtime.CompilerServices;

namespace System.Net.Sockets;

public static class SocketsExtensions
{
	private static readonly TimeSpan _receiveTimeSpan = TimeSpan.FromSeconds(5);

	public static ValueTask<UdpReceiveResult> SendAndReceiveAsync(this UdpClient client, IPEndPoint endPoint, byte[] request, CancellationToken cancellationToken = default)
		=> SendAndReceiveAsyncEnumerable(client, endPoint, request, cancellationToken).FirstOrDefaultAsync(cancellationToken);

	public static async IAsyncEnumerable<UdpReceiveResult> SendAndReceiveAsyncEnumerable(this UdpClient client, IPEndPoint endPoint, byte[] request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await client.SendAsync(request, endPoint, cancellationToken);

		while (!cancellationToken.IsCancellationRequested)
		{
			var task = await Task.WhenAny(
				Task.Delay(_receiveTimeSpan, cancellationToken),
				client.ReceiveAsync(cancellationToken).AsTask());

			if (task is Task<UdpReceiveResult> resultTask)
			{
				var result = await resultTask;
				yield return result;
			}

			if (!cancellationToken.CanBeCanceled) { yield break; }
		}
	}

	public static void Deconstruct(this UdpReceiveResult result, out IPEndPoint endPoint, out byte[] bytes)
	{
		endPoint = result.RemoteEndPoint;
		bytes = result.Buffer;
	}
}
