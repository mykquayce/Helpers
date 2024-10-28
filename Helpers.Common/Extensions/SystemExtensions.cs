namespace System;

public static class SystemExtensions
{
	public static string Truncate(this string? s) => s?.Substring(0, Math.Min(s.Length, 100)) ?? string.Empty;

	public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
		this IDictionary<TKey, TValue> dictionary,
		IEnumerable<KeyValuePair<TKey, TValue>> range)
	{
		ArgumentNullException.ThrowIfNull(dictionary);
		ArgumentNullException.ThrowIfNull(range);

		using var enumerator = range.GetEnumerator();

		while (enumerator.MoveNext())
		{
			var (key, value) = enumerator.Current;

			if (!dictionary.TryAdd(key, value))
			{
				dictionary[key] = value;
			}
		}

		return dictionary;
	}

	public static Uri StripQuery(this Uri uri)
	{
		ArgumentNullException.ThrowIfNull(uri);
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, uri.IsAbsoluteUri);
		ArgumentException.ThrowIfNullOrWhiteSpace(uri.OriginalString);

		return new Uri(uri.GetLeftPart(UriPartial.Path), UriKind.Absolute);
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

	public static IEnumerable<T> Expand<T>(this T @enum) where T : Enum, IConvertible
	{
		var provider = System.Globalization.CultureInfo.InvariantCulture;
		var type = typeof(T);
		var values = (T[])Enum.GetValues(type);
		var enumLong = @enum.ToInt64(provider);

		foreach (var value in values)
		{
			var l = value.ToInt64(provider);

			if (l == enumLong
				|| (l & enumLong) != 0)
			{
				yield return value;
			}
		}
	}
}
