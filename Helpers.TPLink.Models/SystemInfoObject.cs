using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.TPLink.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct SystemInfoObject(
		string alias,
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress mac,
		string model,
		int relay_state);
