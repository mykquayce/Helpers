using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.Networking.Models;

public record DhcpLease(
	DateTime Expiration,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress PhysicalAddress,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonIPAddressConverter))] IPAddress IPAddress,
	string? HostName,
	string? Identifier);
