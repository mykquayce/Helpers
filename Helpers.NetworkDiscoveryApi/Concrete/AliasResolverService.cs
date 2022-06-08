using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi.Concrete;

public class AliasResolverService : IAliasResolverService
{
	private readonly IReadOnlyDictionary<string, PhysicalAddress> _aliases;
	private readonly IService _networkDiscoveryService;

	public AliasResolverService(
		IOptions<Aliases> options,
		IService networkDiscoveryService)
	{
		Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().DoesNotContainNull();

		_aliases = (
			from kvp in options.Value
			let key = kvp.Key
			let value = PhysicalAddress.Parse(kvp.Value)
			select (key, value)
		).ToDictionary(t => t.key, t => t.value, StringComparer.OrdinalIgnoreCase);
		_networkDiscoveryService = Guard.Argument(networkDiscoveryService).NotNull().Value;
	}

	public async Task<IPAddress> ResolveAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var ok = _aliases.TryGetValue(alias, out var physicalAddress);

		if (!ok) throw new KeyNotFoundException($"unknown alias '{alias}'.  known aliases: '{string.Join(',', _aliases.Keys)}'");

		(_, _, var ip, _, _) = await _networkDiscoveryService.GetLeaseAsync(physicalAddress!, cancellationToken);

		return ip;
	}
}
