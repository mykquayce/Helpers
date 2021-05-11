using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public abstract class JsonTypeConverter<T> : JsonConverter<T>
	{
		public abstract Func<string?, T?> Parse { get; }

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Parse(reader.GetString());
		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => throw new NotImplementedException();
	}
}
