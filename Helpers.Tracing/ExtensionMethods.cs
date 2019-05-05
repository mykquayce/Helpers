using OpenTracing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Helpers.Tracing
{
	public static class ExtensionMethods
	{
		public static string ReducePath(this string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return string.Empty;
			}

			var segments = path.Split(Path.DirectorySeparatorChar);

			return segments[segments.Length - 1];
		}

		public static ISpanBuilder BuildDefaultSpan(
			this ITracer tracer,
			[CallerFilePath] string filePath = default,
			[CallerMemberName] string methodName = default)
		{
			var path = filePath.ReducePath();

			var operationName = string.Concat(path, "=>", methodName);

			return tracer.BuildSpan(operationName);
		}

		public static ISpan Log(this ISpan span, params object[] values)
		{
			var dictionary = new Dictionary<string, object>(values.Length / 2);

			for (var a = 0; a < values.Length; a += 2)
			{
				var key = values[a].ToString();
				var value = values[a + 1];

				dictionary[key] = value;
			}

			span.Log(dictionary);

			return span;
		}
	}
}
