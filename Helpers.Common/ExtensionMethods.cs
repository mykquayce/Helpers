using Dawn;
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
			Guard.Argument(() => dictionary).NotNull();

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
			Guard.Argument(() => dictionary).NotNull();
			Guard.Argument(() => range).NotNull();

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
			Guard.Argument(() => uri).NotNull().Require(u => u.IsAbsoluteUri, _ => nameof(uri) + " must be absolute");
			Guard.Argument(() => uri.OriginalString).NotNull().NotEmpty().NotWhiteSpace();

			var normalized = string.Concat(uri.Scheme, Uri.SchemeDelimiter, uri.Host, uri.AbsolutePath);

			return new Uri(normalized, UriKind.Absolute);
		}

		public static int GetDeterministicHashCode(this string s)
		{
			unchecked
			{
				int hash1 = 5_381, hash2 = hash1;

				for (var i = 0; i < s.Length && s[i] != '\0'; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ s[i];
					if (i == s.Length - 1 || s[i + 1] == '\0')
					{
						break;
					}
					hash2 = ((hash2 << 5) + hash2) ^ s[i + 1];
				}

				return hash1 + (hash2 * 1_566_083_941);
			}
		}

		public static IEnumerable<KeyValuePair<object, object?>> GetData(this Exception exception)
		{
			var enumerator = exception.Data.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var key = enumerator.Key;
				var value = enumerator.Value;

				yield return new KeyValuePair<object, object?>(key, value);
			}

			if (exception is AggregateException aggregateException)
			{
				foreach (var innerException in aggregateException.InnerExceptions)
				{
					foreach (var kvp in innerException.GetData())
					{
						yield return kvp;
					}
				}
			}

			if (exception.InnerException is null)
			{
				yield break;
			}

			foreach (var kvp in exception.InnerException.GetData())
			{
				yield return kvp;
			}
		}
	}
}
