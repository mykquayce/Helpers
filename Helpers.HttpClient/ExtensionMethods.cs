using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Helpers.HttpClient
{
	internal static class ExtensionMethods
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			IgnoreNullValues = false,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		internal static string Truncate(this string? s) => s?.Substring(0, Math.Min(s.Length, 100)) ?? string.Empty;

		internal static string Serialize(this object? value)
		{
			if (value == default)
			{
				return "{}";
			}

			try
			{
				return JsonSerializer.ToString(value, _jsonSerializerOptions);
			}
			catch
			{
				return JsonSerializer.ToString(new { value = value.ToString(), }, _jsonSerializerOptions);
			}
		}

		internal static Task<T> Deserialize<T>(this Stream stream) =>
			JsonSerializer.ReadAsync<T>(stream, _jsonSerializerOptions).AsTask();

		internal static string ToKeyValuePairString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			var sb = new StringBuilder();

			using var enumerator = dictionary.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var (key, value) = enumerator.Current;

				sb.AppendFormat("{0}={1};", key, value);
			}

			return sb.ToString();
		}

		internal static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary,
			IEnumerable<KeyValuePair<TKey, TValue>> range)
		{
			using var enumerator = range.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var (key, value) = enumerator.Current;

				dictionary.Add(key, value);
			}

			return dictionary;
		}
	}
}
