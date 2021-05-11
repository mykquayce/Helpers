using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration
{
	public static class ConfigurationExtensions
	{
		/// <summary>
		/// JSON-deserialize a config section into T.  Only works without nested objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static T GetType<T>(this IConfiguration configuration)
		{
			var dictionary = configuration
				.Get<Dictionary<string, string>>()
				.ToStringObjectKeyValuePairs()
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

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

		public static IEnumerable<KeyValuePair<string, object>> ToStringObjectKeyValuePairs(this IEnumerable<KeyValuePair<string, string>> dictionary)
		{
			foreach (var (key, value) in dictionary)
			{
				yield return new(key, value.ToObject());
			}
		}

		public static object ToObject(this string s)
		{
			if (int.TryParse(s, out var i)) return i;
			if (double.TryParse(s, out var d)) return d;
			if (bool.TryParse(s, out var b)) return b;
			return s;
		}
	}
}
