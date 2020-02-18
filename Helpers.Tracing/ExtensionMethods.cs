using Helpers.Common;
using OpenTracing;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Helpers.Tracing
{
	public static class ExtensionMethods
	{
		private readonly static IDictionary<string, string> _textMap = new Dictionary<string, string>(1)
		{
			["message"] = "hello world",
		};

		public static ISpanBuilder BuildDefaultSpan(
			this ITracer tracer,
			[CallerMemberName] string? callerMethodName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			if (tracer is null) throw new ArgumentNullException(nameof(tracer));

			var operationName = GetOperationName(callerMethodName, callerFilePath);

			return tracer.BuildSpan(operationName);
		}

		public static IScope StartParentSpan(
			this ITracer tracer,
			[CallerMemberName] string? callerMethodName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			var operationName = GetOperationName(callerMethodName, callerFilePath);

			var scope = tracer
				.BuildSpan(operationName)
				.StartActive();

			tracer.Inject(
				scope.Span.Context,
				format: BuiltinFormats.TextMap,
				carrier: new TextMapInjectAdapter(_textMap));

			return scope;
		}

		public static IScope StartSpan(
			this ITracer tracer,
			[CallerMemberName] string? callerMethodName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			var operationName = GetOperationName(callerMethodName, callerFilePath);

			var context = tracer.Extract(
				format: BuiltinFormats.TextMap,
				carrier: new TextMapExtractAdapter(_textMap));

			if (context is null)
			{
				return tracer.StartParentSpan(callerMethodName, callerFilePath);
			}

			return tracer.BuildSpan(operationName)
				.AsChildOf(context)
				.StartActive();
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

			var data = exception.GetData();
			exception = exception.GetBaseException();

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
						[nameof(Exception.Data)] = data.ToKeyValuePairString(),
					});
		}

		private static string? GetOperationName(string? callerMethodName, string? callerFilePath)
		{
			var fileName = Path.GetFileNameWithoutExtension(callerFilePath);

			return (fileName, callerMethodName) switch
			{
				(null, null) => default,
				(_, null) => fileName!,
				(null, _) => callerMethodName!,
				_ => string.Concat(fileName!, "=>", callerMethodName!),
			};
		}
	}
}
