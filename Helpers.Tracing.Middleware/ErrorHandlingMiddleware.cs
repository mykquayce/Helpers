using Microsoft.AspNetCore.Http;
using OpenTracing;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.Tracing.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ITracer _tracer;

		public ErrorHandlingMiddleware(
			RequestDelegate next,
			ITracer tracer)
		{
			_next = next;
			_tracer = tracer;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (System.Exception exception)
			{
				string errorObject;

				try { errorObject = JsonSerializer.Serialize(exception); }
				catch { errorObject = exception.ToString(); }

				_tracer?.ActiveSpan?
					.SetTag(OpenTracing.Tag.Tags.Error, true)
					.Log(
						new Dictionary<string, object>(5)
						{
							[LogFields.ErrorKind] = exception.GetType().FullName!,
							[LogFields.ErrorObject] = errorObject,
							[LogFields.Event] = OpenTracing.Tag.Tags.Error.Key,
							[LogFields.Message] = exception.Message,
							[LogFields.Stack] = exception.StackTrace!,
						});
			}
		}
	}
}
