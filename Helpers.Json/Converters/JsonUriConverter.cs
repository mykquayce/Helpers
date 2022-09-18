using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters;

public class JsonUriConverter : JsonConverter<Uri>
{
	public override Uri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var s = reader.GetString();
		return s is not null && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var result)
			? result
			: null;
	}

	public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}
