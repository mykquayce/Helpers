﻿using Dawn;
using System.Text;

namespace System.Collections.Generic;

public static class CollectionExtensions
{
	public static IEnumerator<(TFirst, TSecond)> GetEnumerator<TFirst, TSecond>(this (IEnumerable<TFirst>, IEnumerable<TSecond>) tuple)
	{
		Guard.Argument(tuple).NotDefault();
		var first = Guard.Argument(tuple.Item1).NotNull().Wrap(e => e.GetEnumerator()).NotNull().Value;
		var second = Guard.Argument(tuple.Item2).NotNull().Wrap(e => e.GetEnumerator()).NotNull().Value;

		return new DoubleEnumerator<TFirst, TSecond>(first, second);
	}

	public static string ToKeyValuePairString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
	{
		Guard.Argument(dictionary).NotNull();

		var sb = new StringBuilder();

		using var enumerator = dictionary.GetEnumerator();

		while (enumerator.MoveNext())
		{
			var (key, value) = enumerator.Current;

			sb.AppendFormat("{0}={1};", key, value);
		}

		return sb.ToString();
	}

	public static string ToCsvString(this IDictionary dictionary)
	{
		var sb = new StringBuilder();

		var enumerator = dictionary.GetEnumerator();

		while (enumerator.MoveNext())
		{
			sb.Append($"{enumerator.Key}={enumerator.Value};");
		}

		var s = sb.ToString();

		if (s.Length > 0)
		{
			return s[..^1];
		}

		return string.Empty;
	}

	public static IEnumerable<T> TakeSample<T>(this IEnumerable<T> items, double ratio = .01d)
	{
		var d = 1 / ratio;
		var value = 0d;

		foreach (var item in items)
		{
			value %= d;

			if (value < 1)
			{
				yield return item;
			}

			value++;
		}
	}
}
