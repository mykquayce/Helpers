using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Clients;

public interface IPingClient
{
	Task<Models.PacketLossResults> PacketLossTestAsync(IPAddress ip, int milliseconds = 10000);
	Task<PingReply> PingAsync(IPAddress ip);
	IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, CancellationToken cancellationToken);
	IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, int milliseconds);
}
