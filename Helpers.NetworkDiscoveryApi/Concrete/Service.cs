using Dawn;
using Microsoft.Extensions.Caching.Memory;
using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly IMemoryCache _memoryCache;

	public Service(IClient client, IMemoryCache memoryCache)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
	}

	public async Task<Models.DhcpResponseObject> GetLeaseAsync(PhysicalAddress physicalAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(physicalAddress).NotNull().NotEqual(PhysicalAddress.None);

		if (!_memoryCache.TryGetValue(physicalAddress, out Models.DhcpResponseObject lease))
		{
			lease = await _client.GetLeaseAsync(physicalAddress, cancellationToken);
			_memoryCache.Set(physicalAddress, lease, absoluteExpiration: DateTimeOffset.UtcNow.AddHours(1));
		}

		return lease;
	}

	public async IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default)
	{
		var leases = _client.GetLeasesAsync(cancellationToken);

		await using var enumerator = leases.GetAsyncEnumerator(cancellationToken ?? CancellationToken.None);

		while (await enumerator.MoveNextAsync(cancellationToken ?? CancellationToken.None))
		{
			var lease = enumerator.Current;
			var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
			_memoryCache.Set(lease.physicalAddress, lease, absoluteExpiration);
			yield return lease;
		}
	}
}
