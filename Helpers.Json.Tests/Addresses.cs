using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.Json.Tests
{
	public record Addresses(
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonIPAddressConverter))] IPAddress IPAddress,
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress PhysicalAddress)
	{
		public Addresses() : this(IPAddress.None, PhysicalAddress.None) { }
	}
}
