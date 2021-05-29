﻿using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonIPEndPointConverter : JsonConverter<IPEndPoint>
	{
		public override IPEndPoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return IPEndPoint.TryParse(reader.GetString()!, out var result)
				? result
				: default;
		}

		public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
