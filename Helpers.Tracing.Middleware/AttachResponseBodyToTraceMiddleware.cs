using Microsoft.AspNetCore.Http;
using OpenTracing;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Tracing.Middleware
{
	public class AttachResponseBodyToTraceMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ITracer _tracer;

		public AttachResponseBodyToTraceMiddleware(
			RequestDelegate next,
			ITracer tracer)
		{
			_next = next;
			_tracer = tracer;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var original = context.Response.Body;

			try
			{
				using var stream = new MemoryStream();
				using var reader = new StreamReader(stream);

				context.Response.Body = stream;

				await _next(context);

				stream.Position = 0;

				var body = await reader.ReadToEndAsync();

				_tracer.ActiveSpan?.SetTag("http.response.body", body);

				stream.Position = 0;

				if (!string.IsNullOrEmpty(body))
				{
					await stream.CopyToAsync(original);
				}
			}
			finally
			{
				context.Response.Body = original;
			}
		}
	}
}
