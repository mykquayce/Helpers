using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.NetworkDiscoveryApi.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record DhcpResponseObject(
	DateTime expiration,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress physicalAddress,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonIPAddressConverter))] IPAddress ipAddress,
	string? hostName,
	string? identifier);
