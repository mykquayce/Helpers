using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models
{

	public record ArpResult(PhysicalAddress PhysicalAddress, IPAddress IPAddress, ArpResult.Types Type)
	{
		[Flags]
		public enum Types
		{
			None = 0,
			Dynamic = 1,
			Static = 2,
		}
	}
}
