using System.Text.Json.Serialization;

namespace Helpers.Networking.Models
{
	public record WhoIsResponse(
		[property: JsonConverter(typeof(JsonAddressPrefixConverter))] AddressPrefix Prefix,
		int ASN, string Description, int NumRisPeers);
}
