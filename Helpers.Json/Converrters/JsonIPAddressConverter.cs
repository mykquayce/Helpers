using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonIPAddressConverter : JsonConverter<IPAddress>
	{
		public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return IPAddress.TryParse(reader.GetString(), out var result)
				? result
				: default;
		}

		public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString().ToLowerInvariant());
		}
	}
}
