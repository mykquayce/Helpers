using System;
using System.Net;

namespace Helpers.Json.Converters
{
	public class JsonIPAddressConverter : JsonTypeConverter<IPAddress>
	{
		public override Func<string?, IPAddress?> Parse { get; } = s => IPAddress.TryParse(s, out var result) ? result : default;
	}
}
