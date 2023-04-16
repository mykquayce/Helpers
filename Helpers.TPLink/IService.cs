using Helpers.TPLink.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink;

public interface IService
{
	IAsyncEnumerable<(string, IPEndPoint, PhysicalAddress)> DiscoverAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default);
	Task<(double amps, double volts, double watts)> GetRealtimeDataAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default);
	Task<bool> GetStateAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default);
	Task<SystemInfoObject> GetSystemInfoAsync(IPEndPoint endPoint, CancellationToken cancellationToken = default);
	Task SetStateAsync(IPEndPoint endPoint, bool on, CancellationToken cancellationToken = default);

	Task<(double amps, double volts, double watts)> GetRealtimeDataAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
		=> GetRealtimeDataAsync(new IPEndPoint(ipAddress, Constants.Port), cancellationToken);
	Task<bool> GetStateAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
		=> GetStateAsync(new IPEndPoint(ipAddress, Constants.Port), cancellationToken);
	Task<SystemInfoObject> GetSystemInfoAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
		=> GetSystemInfoAsync(new IPEndPoint(ipAddress, Constants.Port), cancellationToken);
	Task SetStateAsync(IPAddress ipAddress, bool on, CancellationToken cancellationToken = default)
		=> SetStateAsync(new IPEndPoint(ipAddress, Constants.Port), on, cancellationToken);
}
