using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models.Extensions
{
	public static class StringExtensions
	{
		public static IPAddress ParseIPAddress(this string ipAddressString)
		{
			return IPAddress.TryParse(ipAddressString, out var ipAddress)
				? ipAddress
				: throw new ArgumentOutOfRangeException(nameof(ipAddressString), ipAddressString, $"Unexpected {nameof(ipAddressString)}: {ipAddressString}");
		}

		public static PhysicalAddress ParsePhysicalAddress(this string physicalAddressString)
		{
			return PhysicalAddress.TryParse(physicalAddressString, out var physicalAddress)
				? physicalAddress
				: throw new ArgumentOutOfRangeException(nameof(physicalAddressString), physicalAddressString, $"Unexpected {nameof(physicalAddressString)}: {physicalAddressString}");
		}
	}
}
