using Helpers.Json.Extensions;
using System.Collections.Generic;
using System.Linq;

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
		public static T JsonConfig<T>(this IConfiguration configuration)
			=> configuration.ToDictionary().ToType<T>();

		public static System.Collections.IDictionary ToDictionary(this IConfiguration configuration)
		{
			return configuration
				.Get<Dictionary<string, string>>()
				.ToStringObjectKeyValuePairs()
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
