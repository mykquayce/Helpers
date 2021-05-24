using OpenTracing;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class BuildSpanTests : IClassFixture<Fixtures.TracerFixture>
	{
		private readonly ITracer _tracer;

		public BuildSpanTests(Fixtures.TracerFixture fixture)
		{
			_tracer = fixture.Tracer;
		}

		[Fact]
		public void BuildSpanTests_BuildOneSpan()
		{
			using var scope1 = _tracer
				.BuildSpan("outer-operation1")
				.WithTag("key", "value1")
				.StartActive(finishSpanOnDispose: true);

			scope1.Span.Log("hello1");

			System.Threading.Thread.Sleep(millisecondsTimeout: 100);
			System.Threading.Thread.Sleep(millisecondsTimeout: 100);

			scope1.Span.Finish();
		}

		[Theory]
		[InlineData("operation-name", "tag", "message")]
		public void BuildSpanTests_Go(string operationName, string tag, string message)
		{
			var span = _tracer
				.BuildSpan(operationName)
				.WithTag(nameof(tag), tag)
				.Start();

			using var scope = _tracer.ScopeManager.Activate(span, finishSpanOnDispose: true);

			span.Log(message);

			span.Finish();
		}
	}
}
