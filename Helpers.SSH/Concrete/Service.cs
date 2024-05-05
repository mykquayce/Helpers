using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace Helpers.SSH.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly string _newline;

	public Service(IClient client)
	{
		ArgumentNullException.ThrowIfNull(client);
		_client = client;
		_newline = _client.RunCommandAsync("echo").GetAwaiter().GetResult();
	}

	public async IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var command = "(ip route show && ip -6 route show) | grep ^[Bb]lackhole | awk '{print($2)}'";

		var response = await _client.RunCommandAsync(command, cancellationToken);

		var lines = response.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines)
		{
			yield return Helpers.Networking.Models.AddressPrefix.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
		}
	}

	public Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(subnetAddress);
		return _client.RunCommandAsync("ip route add blackhole " + subnetAddress, cancellationToken);
	}

	public Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> subnetAddresses, CancellationToken cancellationToken = default)
		=> Task.WhenAll(subnetAddresses.Select(p => AddBlackholeAsync(p, cancellationToken)));

	public Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(subnetAddress);
		return _client.RunCommandAsync("ip route delete blackhole " + subnetAddress, cancellationToken);
	}

	public Task DeleteBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> subnetAddresses, CancellationToken cancellationToken = default)
		=> Task.WhenAll(subnetAddresses.Select(p => DeleteBlackholeAsync(p, cancellationToken)));

	public Task DeleteBlackholesAsync(CancellationToken cancellationToken = default)
		=> _client.RunCommandAsync("(ip route show && ip -6 route show) | grep ^blackhole | awk '{system(\"ip route delete blackhole \" $2)}'", cancellationToken);

	public async IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var output = await _client.RunCommandAsync("cat /tmp/dhcp.leases", cancellationToken);

		var lines = output.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines) yield return GetDhcpLease(line);
	}

	public static Helpers.Networking.Models.DhcpLease GetDhcpLease(string dhcpLeaseString)
	{
		ArgumentException.ThrowIfNullOrEmpty(dhcpLeaseString);

		var values = dhcpLeaseString.Split(' ', count: 5);
		ArgumentOutOfRangeException.ThrowIfNotEqual(values.Length, 5, nameof(dhcpLeaseString));

		var expiration = DateTime.UnixEpoch.AddSeconds(int.Parse(values[0]));
		var physicalAddress = PhysicalAddress.Parse(values[1]);
		var ipAddress = IPAddress.Parse(values[2]);
		var hostName = values[3] == "*" ? default : values[3];
		var identifier = values[4] == "*" ? default : values[4];

		return new(expiration, physicalAddress, ipAddress, hostName, identifier);
	}

	public async Task<Helpers.Networking.Models.DhcpLease> GetLeaseByIPAddressAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
	{
		await foreach (var lease in GetDhcpLeasesAsync(cancellationToken))
		{
			if (Equals(lease.IPAddress, ipAddress))
			{
				return lease;
			}
		}

		throw new KeyNotFoundException($"{nameof(ipAddress)} {ipAddress} not found");
	}

	public async Task<Helpers.Networking.Models.DhcpLease> GetLeaseByPhysicalAddressAsync(PhysicalAddress physicalAddress, CancellationToken cancellationToken = default)
	{
		await foreach (var lease in GetDhcpLeasesAsync(cancellationToken))
		{
			if (Equals(lease.PhysicalAddress, physicalAddress))
			{
				return lease;
			}
		}

		throw new KeyNotFoundException($"{nameof(physicalAddress)} {physicalAddress} not found");
	}
}
