using System.Net;
using System.Text.Json.Serialization;

namespace Helpers.GlobalCache
{
	public record Config(
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonIPAddressConverter))] IPAddress BroadcastIPAddress,
		ushort BroadcastPort,
		ushort ReceivePort)
	{
		public static readonly IPAddress DefaultBroadcastIPAddress = IPAddress.Parse("239.255.250.250");
		public const ushort DefaultBroadcastPort = 4_998;
		public const ushort DefaultReceivePort = 9_131;

		public Config() : this(DefaultBroadcastIPAddress, DefaultBroadcastPort, DefaultReceivePort) { }

		public static Config Defaults => new();
	}
}
