﻿using OpenTracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Helpers.Tracing
{
	public static class ExtensionMethods
	{
		public static ISpanBuilder BuildDefaultSpan(
			this ITracer tracer,
			[CallerFilePath] string? filePath = default,
			[CallerMemberName] string? methodName = default)
		{
			if (tracer is null) throw new ArgumentNullException(nameof(tracer));

			var fileName = Path.GetFileNameWithoutExtension(filePath);

			var operationName = (fileName, methodName) switch
			{
				(null, null) => default,
				(_, null) => fileName,
				(null, _) => methodName,
				_ => string.Concat(fileName, "=>", methodName)
			};

			return tracer.BuildSpan(operationName);
		}

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

			return span
				.SetTag(OpenTracing.Tag.Tags.Error, true)
				.Log(
					new Dictionary<string, object?>(5)
					{
						[LogFields.ErrorKind] = exception.GetType().FullName,
						[LogFields.ErrorObject] = exception,
						[LogFields.Event] = OpenTracing.Tag.Tags.Error.Key,
						[LogFields.Message] = exception.Message,
						[LogFields.Stack] = exception.StackTrace,
					});
		}
	}
}
