using Microsoft.Extensions.DependencyInjection;
using OpenTracing;
using System;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class JaegerTracerTests : IDisposable
	{
		private readonly ITracer _tracer;
		private readonly IServiceProvider _serviceProvider;

		public JaegerTracerTests()
		{
			var settings = new Helpers.Jaeger.Models.Settings
			{
				ServiceName = "jaeger-tests",
				Host = "localhost",
				Port = 6_831,
			};

			var services = new ServiceCollection()
				.AddJaegerTracing(settings);

			_serviceProvider = services
				.BuildServiceProvider();

			_tracer = _serviceProvider.GetRequiredService<ITracer>();
		}

		[Theory]
		[InlineData("operation-name", "hello world")]
		public void ExtensionMethod(string operationName, string message)
		{
			using var scope = _tracer.BuildSpan(operationName)
				.WithTag(new OpenTracing.Tag.StringTag("message"), message)
				.StartActive(finishSpanOnDispose: true);
		}

		#region IDisposable implementation
		public void Dispose()
		{
			(_tracer as IDisposable)?.Dispose();
			(_serviceProvider as IDisposable)?.Dispose();
		}
		#endregion IDisposable implementation
	}
}
