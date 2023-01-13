using System.Text.Json.Serialization;

namespace Helpers.OldhamCouncil.Models;

public record Address(
	[property: JsonPropertyName("UPRN")] string Uprn,
	[property: JsonPropertyName("FULL_ADDRESS")] string FullAddress);
