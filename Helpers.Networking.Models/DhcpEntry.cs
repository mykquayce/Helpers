using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models
{
	public record DhcpEntry(DateTime Expiration, PhysicalAddress PhysicalAddress, IPAddress IPAddress, string? HostName, string? Identifier)
	{
		public static DhcpEntry Parse(string s)
		{
			var values = s.Split(' ');
			var expires = DateTime.UnixEpoch.AddSeconds(int.Parse(values[0]));
			var physicalAddress = PhysicalAddress.Parse(values[1]);
			var ipAddress = IPAddress.Parse(values[2]);
			var hostName = values[3] == "*" ? default : values[3];
			var identifier = values[4] == "*" ? default : values[4];

			return new DhcpEntry(expires, physicalAddress, ipAddress, hostName, identifier);
		}
	}
}
