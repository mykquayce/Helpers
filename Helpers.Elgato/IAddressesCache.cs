using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Elgato
{
	public interface IAddressesCache
	{
		void Set(PhysicalAddress physicalAddress, IPAddress ipAddress, DateTimeOffset expiration);
		bool TryGet(PhysicalAddress physicalAddress, out IPAddress? ipAddress);
	}
}
