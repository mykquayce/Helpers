using Microsoft.AspNetCore.Http;
using OpenTracing;
using System.IO;
using System.Text;
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

			using var memoryStream = new MemoryStream();

			using var _ = new Swap<Stream>(() => context.Response.Body, stream => context.Response.Body = stream, memoryStream);

			await _next.Invoke(context);

			memoryStream.Position = 0L;

			await SaveAsync(memoryStream);

			memoryStream.Position = 0L;

			await memoryStream.CopyToAsync(original);
		}

		private async Task SaveAsync(Stream stream)
		{
			if (_tracer?.ActiveSpan == default
				|| stream == default
				|| !stream.CanRead
				|| !stream.CanSeek)
			{
				return;
			}

			string body;

			using (var reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true, bufferSize: 1, leaveOpen: true))
			{
				body = await reader.ReadToEndAsync();
			}

			_tracer.ActiveSpan.SetTag("http.response.body", body);
		}
	}
}
