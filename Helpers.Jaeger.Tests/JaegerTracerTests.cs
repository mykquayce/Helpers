using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class JaegerTracerTests
	{
		[Theory]
		[InlineData("operation-name", "jaeger-tests", "localhost", 6_831, "hello world")]
		public void JaegerTracerTests_BehavesPredictably(string operationName, string serviceName, string host, int port, string message)
		{
			var settings = new Helpers.Jaeger.Models.Settings
			{
				ServiceName = serviceName,
				Host = host,
				Port = port,
			};

			var tracer = new Helpers.Jaeger.JaegerTracer(settings);

			using var scope = tracer.BuildSpan(operationName)
				.WithTag(new OpenTracing.Tag.StringTag("message"), message)
				.StartActive(finishSpanOnDispose: true);
		}
	}
}
