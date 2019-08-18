using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Common
{
	public static class ExtensionMethods
	{
		public static string Truncate(this string? s) => s?.Substring(0, Math.Min(s.Length, 100)) ?? string.Empty;

		public static string ToKeyValuePairString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
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

		public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
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

		public static Uri StripQuery(this Uri uri)
		{
			if (uri?.OriginalString == default
				|| string.IsNullOrWhiteSpace(uri.OriginalString))
			{
				throw new ArgumentNullException(nameof(uri));
			}

			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentOutOfRangeException(nameof(uri), uri, nameof(uri) + " must be absolute")
				{
					Data = { [nameof(uri)] = uri.OriginalString, },
				};
			}

			var normalized = string.Concat(uri.Scheme, Uri.SchemeDelimiter, uri.Host, uri.AbsolutePath);

			return new Uri(normalized, UriKind.Absolute);
		}
	}
}
