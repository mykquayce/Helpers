using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models
{
	public record DhcpLease(DateTime Expiration, PhysicalAddress PhysicalAddress, IPAddress IPAddress, string? HostName, string? Identifier);
}
