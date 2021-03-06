﻿using Dawn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

				if (!dictionary.TryAdd(key, value))
				{
					dictionary[key] = value;
				}
			}

			return dictionary;
		}

		public static Uri StripQuery(this Uri uri)
		{
			Guard.Argument(() => uri).NotNull().Wrap(u => u.IsAbsoluteUri)
				.Positive(_ => nameof(uri) + " must be absolute");

			Guard.Argument(() => uri).NotNull().Wrap(u => u.OriginalString)
				.NotNull().NotEmpty().NotWhiteSpace();

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

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfosLeafFirst(this DirectoryInfo dir, string searchPattern = "*.*")
		{
			foreach (var fsi in dir.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
			{
				if (fsi is DirectoryInfo subDirectory)
				{
					foreach (var nested in subDirectory.EnumerateFileSystemInfosLeafFirst(searchPattern))
					{
						yield return nested;
					}
				}

				yield return fsi;
			}
		}

		public async static Task<T> GetAsync<T>(this HttpClient client, Uri requestUri, CancellationToken? cancellationToken = default)
		{
			using var response = await client.GetAsync(requestUri, cancellationToken: cancellationToken ?? CancellationToken.None);

			using var stream = await response.Content.ReadAsStreamAsync();

			try
			{
				return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken ?? CancellationToken.None);
			}
			catch (Exception ex)
			{
				stream.Position = 0L;

				using var reader = new StreamReader(stream);

				var text = await reader.ReadToEndAsync();

				ex.Data.Add(nameof(text), text);

				throw;
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
}
