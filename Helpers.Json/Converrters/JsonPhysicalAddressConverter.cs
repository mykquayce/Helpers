using System;
using System.Net.NetworkInformation;

namespace Helpers.Json.Converters
{
	public class JsonPhysicalAddressConverter : JsonTypeConverter<PhysicalAddress>
	{
		public override Func<string?, PhysicalAddress?> Parse { get; } = s => PhysicalAddress.TryParse(s, out var result) ? result : default;
	}
}
