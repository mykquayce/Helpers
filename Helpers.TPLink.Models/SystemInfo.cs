using System.Net.NetworkInformation;

namespace Helpers.TPLink.Models
{
	public record SystemInfo(string Model, string Alias, PhysicalAddress PhysicalAddress)
	{
		public static explicit operator SystemInfo(Generated.ResponseObject.SystemObject.SystemInfoObject generated)
			=> new(generated.model, generated.alias, generated.mac);
	}
}
