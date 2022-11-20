using System.Net;

namespace Helpers.TPLink;

public interface IClient
{
	IAsyncEnumerable<Models.RealtimeInfoObject> GetRealtimeDataAsync(IPAddress ip, CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.SystemInfoObject> GetSystemInfoAsync(IPAddress ip, CancellationToken cancellationToken = default);
	IAsyncEnumerable<bool> GetStateAsync(IPAddress ip, CancellationToken cancellationToken = default)
		=> GetSystemInfoAsync(ip, cancellationToken).Select(info => info.relay_state == 1);
	ValueTask<int> SetStateAsync(IPAddress ip, bool state, CancellationToken cancellationToken = default);
}
