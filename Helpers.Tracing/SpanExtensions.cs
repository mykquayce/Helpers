using Helpers.Common;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Helpers.Tracing
{
	public static class SpanExtensions
	{
		public static ISpan Log(this ISpan span, params (string, object?)[] keyValuePairs)
		{
			if (span is null) throw new ArgumentNullException(nameof(span));

			var dictionary = new Dictionary<string, object?>(keyValuePairs.Length / 2);

			foreach (var (key, value) in keyValuePairs)
			{
				dictionary.Add(key, value);
			}

			span.Log(dictionary);

			return span;
		}

		public static ISpan Log(this ISpan span, params object?[] values)
		{
			if (span is null) throw new ArgumentNullException(nameof(span));

			var dictionary = new Dictionary<string, object?>(values.Length / 2);

			for (var a = 0; a < values.Length; a += 2)
			{
				var key = values[a]!.ToString();
				var value = values[a + 1];

				dictionary.Add(key, value);
			}

			span.Log(dictionary);

			return span;
		}

		public static ISpan Log(this ISpan span, Exception exception)
		{
			if (span is null) throw new ArgumentNullException(nameof(span));
			if (exception is null) throw new ArgumentNullException(nameof(exception));

			var data = exception.GetData();
			exception = exception.GetBaseException();

			return span
				.SetTag(OpenTracing.Tag.Tags.Error, true)
				.Log(
					new Dictionary<string, object?>(6)
					{
						[LogFields.ErrorKind] = exception.GetType().FullName,
						[LogFields.ErrorObject] = exception,
						[LogFields.Event] = OpenTracing.Tag.Tags.Error.Key,
						[LogFields.Message] = exception.Message,
						[LogFields.Stack] = exception.StackTrace,
						[nameof(Exception.Data)] = data.ToKeyValuePairString(),
					});
		}

		private static readonly IDictionary<Type, ICollection<PropertyInfo>> _cache = new Dictionary<Type, ICollection<PropertyInfo>>();

		public static ISpan Log<T>(this ISpan span, T? o)
			where T : class
		{
			if (o is null) return span;

			if (o is Exception exception)
			{
				return Log(span, exception);
			}

			var type = typeof(T);

			if (!_cache.TryGetValue(type, out var properties))
			{
				properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

				_cache.Add(type, properties);
			}

			var dictionary = properties
				.ToDictionary(p => p.Name, p => p.GetValue(o));

			return span
				.Log(dictionary);
		}
	}
}
