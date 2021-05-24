using Microsoft.Extensions.DependencyInjection;
using OpenTracing;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class JaegerTracerTests : IClassFixture<Fixtures.ServiceProviderFixture>
	{
		private readonly ITracer _tracer;

		public JaegerTracerTests(Fixtures.ServiceProviderFixture fixture)
		{
			_tracer = fixture.ServiceProvider.GetRequiredService<ITracer>();
		}

		[Theory]
		[InlineData("operation-name", "hello world")]
		public void ExtensionMethod(string operationName, string message)
		{
			using var scope = _tracer.BuildSpan(operationName)
				.WithTag(new OpenTracing.Tag.StringTag("message"), message)
				.StartActive(finishSpanOnDispose: true);
		}
	}
}
