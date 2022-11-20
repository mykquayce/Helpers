using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink;

public interface IDiscoveryClient
{
	async IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync()
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 2_000);
		await foreach (var tuple in DiscoverAsync(cts.Token))
		{
			yield return tuple;
		}
	}

	IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync(CancellationToken cancellationToken);
}
