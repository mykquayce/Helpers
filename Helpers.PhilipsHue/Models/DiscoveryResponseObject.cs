using System.Net;
using System.Text.Json.Serialization;
using Helpers.PhilipsHue.Converters;

namespace Helpers.PhilipsHue.Models;

public record DiscoveryResponseObject(
	[property: JsonConverter(typeof(JsonHexConverter<ulong>))] ulong Id,
	[property: JsonConverter(typeof(Json.Converters.JsonIPAddressConverter))] IPAddress InternalIPAddress,
	ushort Port);
