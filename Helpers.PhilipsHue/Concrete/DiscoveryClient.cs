using Dawn;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Helpers.PhilipsHue.Concrete;

public class DiscoveryClient : Helpers.Web.WebClientBase, IDiscoveryClient
{
	private const string _cacheKey = "philips-hue-bridge-ip-address";
	private readonly IMemoryCache _memoryCache;

	public DiscoveryClient(IMemoryCache memoryCache, HttpClient httpClient)
		: base(httpClient)
	{
		_memoryCache = Guard.Argument(memoryCache).NotIn().Value;
	}

	public async Task<IPAddress> GetBridgeIPAddressAsync(CancellationToken? cancellationToken = null)
	{
		var ok = _memoryCache.TryGetValue<IPAddress>(_cacheKey, out var ip);
		if (ok) { return ip!; }

		ip = await GetBridgeIPAddressFromRemoteAsync(cancellationToken);
		var expiry = DateTime.UtcNow.AddDays(1);
		_memoryCache.Set(_cacheKey, ip, expiry);
		return ip;
	}

	private async Task<IPAddress> GetBridgeIPAddressFromRemoteAsync(CancellationToken? cancellationToken = null)
	{
		var requestUri = new Uri(string.Empty, UriKind.Relative);
		var response = await base.SendAsync<IReadOnlyList<Models.DiscoveryResponseObject>>(HttpMethod.Get, requestUri, cancellationToken: cancellationToken);
		if (response.StatusCode == HttpStatusCode.OK)
		{
			return response.Object![0].InternalIPAddress;
		}
		throw new WebException(response.StatusCode.ToString("G"));
	}
}
