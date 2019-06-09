using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using OpenTracing;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Tracing.Middleware
{
	public class AttachRequestBodyToTraceMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ITracer _tracer;

		public AttachRequestBodyToTraceMiddleware(
			RequestDelegate next,
			ITracer tracer)
		{
			_next = next;
			_tracer = tracer;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (_tracer?.ActiveSpan == default)
			{
				return;
			}

			BufferingHelper.EnableRewind(context.Request);

			string body;

			using (var reader = new System.IO.StreamReader(context.Request.Body, Encoding.Default, detectEncodingFromByteOrderMarks: true, bufferSize: 1, leaveOpen: true))
			{
				body = await reader.ReadToEndAsync();
				context.Request.Body.Position = 0L;
			}

			_tracer.ActiveSpan.SetTag("http.request.body", body);

			await _next.Invoke(context);
		}
	}
}
