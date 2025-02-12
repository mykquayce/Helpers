﻿using Helpers.Networking.Models;

namespace System.Text.Json.Serialization;

public class JsonAddressPrefixConverter : JsonConverter<AddressPrefix>
{
	public override AddressPrefix? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var s = reader.GetString();
		ArgumentException.ThrowIfNullOrWhiteSpace(s);

		return AddressPrefix.Parse(s!, null);
	}

	public override void Write(Utf8JsonWriter writer, AddressPrefix value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}
