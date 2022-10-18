using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.PhilipsHue.Converters;

public class JsonHexConverter<T> : JsonConverter<T>
	where T : INumber<T>
{
	private readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var ok = T.TryParse(reader.GetString(), NumberStyles.HexNumber, _formatProvider, out var value);
		return ok ? value : default;
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString("x", _formatProvider));
	}
}
