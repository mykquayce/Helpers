using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonBigIntegerConverter : JsonConverter<BigInteger>
	{
		public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return BigInteger.TryParse(reader.GetString(), out var result)
				? result
				: default;
		}

		public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
