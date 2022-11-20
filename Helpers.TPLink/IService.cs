using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink;

public interface IService
{
	IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync();
	IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<(double amps, double volts, double watts)> GetRealtimeDataAsync(IPAddress ip, CancellationToken cancellationToken = default);
	IAsyncEnumerable<bool> GetStateAsync(IPAddress ip, CancellationToken cancellationToken = default);
	ValueTask<int> SetStateAsync(IPAddress ip, bool state, CancellationToken cancellationToken = default);
}
