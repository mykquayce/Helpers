using System;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonPhysicalAddressConverter : JsonConverter<PhysicalAddress>
	{
		public override PhysicalAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			try { return PhysicalAddress.Parse(reader.GetString()); }
			catch (FormatException) { return default; }
		}

		public override void Write(Utf8JsonWriter writer, PhysicalAddress value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString().ToLowerInvariant());
		}
	}
}
