using Dawn;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace Helpers.TPLink.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly IDiscoveryClient _discoveryClient;

	public Service(IClient client, IDiscoveryClient discoveryClient)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_discoveryClient = Guard.Argument(discoveryClient).NotNull().Value;
	}

	public IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync()
		=> _discoveryClient.DiscoverAsync();

	public IAsyncEnumerable<(string, IPAddress, PhysicalAddress)> DiscoverAsync(CancellationToken cancellationToken = default)
		=> _discoveryClient.DiscoverAsync(cancellationToken);

	public async IAsyncEnumerable<(double amps, double volts, double watts)> GetRealtimeDataAsync(
		IPAddress ip,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var data = _client.GetRealtimeDataAsync(ip, cancellationToken);
		await foreach (var power in data)
		{
			var amps = power.current_ma / 1_000d;
			var volts = power.voltage_mv / 1_000d;
			var watts = power.power_mw / 1_000d;
			yield return (amps, volts, watts);
		}
	}

	public IAsyncEnumerable<bool> GetStateAsync(IPAddress ip, CancellationToken cancellationToken = default)
		=> _client.GetStateAsync(ip, cancellationToken);

	public ValueTask<int> SetStateAsync(IPAddress ip, bool state, CancellationToken cancellationToken = default)
		=> _client.SetStateAsync(ip, state, cancellationToken);
}
