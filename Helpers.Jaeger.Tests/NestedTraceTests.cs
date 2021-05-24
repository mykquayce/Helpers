using OpenTracing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public sealed class NestedTraceTests : IClassFixture<Fixtures.TracerFixture>
	{
		private readonly ITracer _tracer;

		public NestedTraceTests(Fixtures.TracerFixture fixture)
		{
			_tracer = fixture.Tracer;
		}

		[Fact]
		public async Task NestedTraceTests_BehavesPredictably()
		{
			using IScope parentScope = _tracer.BuildSpan("Parent").StartActive(finishSpanOnDispose: true);

			await SomeAsynchronousWork();

			// It's still possible to access the current span
			parentScope.Span.Log(new Dictionary<string, object> { ["one"] = "one", });

			// The child scope will automatically use parentScope as its parent.
			using IScope childScope = _tracer.BuildSpan("Child").StartActive(finishSpanOnDispose: true);

			childScope.Span.Log(new Dictionary<string, object> { ["two"] = "two", });

			await SomeMoreAsynchronousWork();

			childScope.Span.Log(new Dictionary<string, object> { ["three"] = "three", });
		}

		public Task SomeAsynchronousWork()
		{
			_tracer.ActiveSpan.Log(nameof(SomeAsynchronousWork));
			return Task.CompletedTask;
		}

		public Task SomeMoreAsynchronousWork()
		{
			_tracer.ActiveSpan.Log(nameof(SomeMoreAsynchronousWork));
			return Task.CompletedTask;
		}
	}
}
