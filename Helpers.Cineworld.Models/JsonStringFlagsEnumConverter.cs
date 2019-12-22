using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Cineworld.Models
{
	public class JsonStringFlagsEnumConverter<T> : JsonConverter<T>
		where T : struct
	{
		private readonly Type _type;
		private readonly IDictionary<string, int> _names = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

		public JsonStringFlagsEnumConverter()
		{
			_type = typeof(T);
			var names = Enum.GetNames(_type);

			foreach (var name in names)
			{
				var t = Enum.Parse<T>(name);
				var value = Convert.ToInt32(t);
				_names.Add(name, value);
			}
		}

		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var s = reader.GetString();
			var names = s.Split('|', ',', ' ', ';');
			var result = 0;

			foreach (var name in names)
			{
				result |= _names[name];
			}

			var o = Enum.ToObject(_type, result);
			return (T)Convert.ChangeType(o, _type);
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
			=> throw new NotImplementedException();
	}
}
