using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonDirectoryInfoConverter : JsonConverter<DirectoryInfo>
	{
		public override DirectoryInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> new(reader.GetString());

		public override void Write(Utf8JsonWriter writer, DirectoryInfo value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.FullName);
	}
}
