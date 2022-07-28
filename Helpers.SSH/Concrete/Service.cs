using Dawn;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.SSH.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly string _newline;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_newline = _client.RunCommandAsync("echo").GetAwaiter().GetResult();
	}

	public async IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync()
	{
		var command = "(ip route show && ip -6 route show) | grep ^[Bb]lackhole | awk '{print($2)}'";

		var response = await _client.RunCommandAsync(command);

		var lines = response.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines)
		{
			yield return Helpers.Networking.Models.AddressPrefix.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
		}
	}

	public Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress)
	{
		Guard.Argument(subnetAddress).NotNull();
		return _client.RunCommandAsync("ip route add blackhole " + subnetAddress);
	}

	public Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> subnetAddresses)
		=> Task.WhenAll(subnetAddresses.Select(AddBlackholeAsync));

	public Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress)
	{
		Guard.Argument(subnetAddress).NotNull();
		return _client.RunCommandAsync("ip route delete blackhole " + subnetAddress);
	}

	public Task DeleteBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> subnetAddresses)
		=> Task.WhenAll(subnetAddresses.Select(DeleteBlackholeAsync));

	public Task DeleteBlackholesAsync()
		=> _client.RunCommandAsync("(ip route show && ip -6 route show) | grep ^blackhole | awk '{system(\"ip route delete blackhole \" $2)}'");

	public async IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync()
	{
		var output = await _client.RunCommandAsync("cat /tmp/dhcp.leases");

		var lines = output.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines) yield return GetDhcpLease(line);
	}

	public static Helpers.Networking.Models.DhcpLease GetDhcpLease(string dhcpLeaseString)
	{
		Guard.Argument(dhcpLeaseString)
			.NotNull()
			.NotEmpty()
			.NotWhiteSpace()
			.Matches(@"^\d+ [\d\w:]+ [\d\.]+ .+? .+?$");

		var values = dhcpLeaseString.Split(' ');

		var expiration = DateTime.UnixEpoch.AddSeconds(int.Parse(values[0]));
		var physicalAddress = PhysicalAddress.Parse(values[1]);
		var ipAddress = IPAddress.Parse(values[2]);
		var hostName = values[3] == "*" ? default : values[3];
		var identifier = values[4] == "*" ? default : values[4];

		return new(expiration, physicalAddress, ipAddress, hostName, identifier);
	}

	public async Task<Helpers.Networking.Models.DhcpLease> GetLeaseByIPAddressAsync(IPAddress ipAddress)
	{
		await foreach (var lease in GetDhcpLeasesAsync())
		{
			if (Equals(lease.IPAddress, ipAddress))
			{
				return lease;
			}
		}

		throw new KeyNotFoundException($"{nameof(ipAddress)} {ipAddress} not found");
	}

	public async Task<Helpers.Networking.Models.DhcpLease> GetLeaseByPhysicalAddressAsync(PhysicalAddress physicalAddress)
	{
		await foreach (var lease in GetDhcpLeasesAsync())
		{
			if (Equals(lease.PhysicalAddress, physicalAddress))
			{
				return lease;
			}
		}

		throw new KeyNotFoundException($"{nameof(physicalAddress)} {physicalAddress} not found");
	}

	public async IAsyncEnumerable<KeyValuePair<PhysicalAddress, IPAddress>> GetArpTableAsync(CancellationToken? cancellationToken = null)
	{
		var linkLocal = Networking.Models.AddressPrefix.Parse("169.254.0.0/16", null);
		var lines = _client.RunCommandAsShellAsync("arp -a", cancellationToken);

		await foreach (var line in lines)
		{
			var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			(var ipString, _, _, var macString, _, _) = values;

			if (IPAddress.TryParse(ipString, out var ip)
				&& !linkLocal.Contains(ip)
				&& PhysicalAddress.TryParse(macString, out var mac))
			{
				yield return new(mac, ip);
			}
		}
	}
}
