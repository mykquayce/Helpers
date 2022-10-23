using OpenTracing;
using OpenTracing.Propagation;
using System.Runtime.CompilerServices;

namespace Helpers.Tracing;

public static class TracerExtensions
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
