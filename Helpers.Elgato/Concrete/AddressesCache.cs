using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Caching;

namespace Helpers.Elgato.Concrete
{
	public class AddressesCache : IAddressesCache
	{
		private readonly ObjectCache _cache = MemoryCache.Default;

		public void Set(PhysicalAddress physicalAddress, IPAddress ipAddress, DateTimeOffset expiration)
		{
			_cache.Set(physicalAddress.ToString().ToLowerInvariant(), ipAddress, expiration);
		}

		public bool TryGet(PhysicalAddress physicalAddress, out IPAddress? ipAddress)
		{
			ipAddress = _cache.Get(physicalAddress.ToString().ToLowerInvariant()) as IPAddress;
			return ipAddress is not null;
		}
	}
}
