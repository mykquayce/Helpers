using System;
using System.Collections;
using System.Text.Json;

namespace Helpers.Json.Extensions
{
	public static class DictionaryExtensions
	{
		public static T ToType<T>(this IDictionary dictionary)
		{
			var json = JsonSerializer.Serialize(dictionary);

			try
			{
				return JsonSerializer.Deserialize<T>(json)
					?? throw new JsonException("Deserialize returned null");
			}
			catch (JsonException ex)
			{
				ex.Data.Add(nameof(json), json);
				ex.Data.Add(nameof(Type), typeof(T));
				throw;
			}
		}
	}
}
