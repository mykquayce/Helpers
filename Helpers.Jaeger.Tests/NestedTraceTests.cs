using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public sealed class NestedTraceTests : IDisposable
	{
		private readonly ITracer _tracer;

		public NestedTraceTests()
		{
			var sender = new UdpSender("localhost", 6_831, maxPacketSize: 0);
			var reporter = new RemoteReporter.Builder().WithSender(sender).Build();
			var sampler = new ConstSampler(sample: true);
			_tracer = new Tracer.Builder("jaeger-tests").WithReporter(reporter).WithSampler(sampler).Build();
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

		public void Dispose()
		{
			(_tracer as Tracer)?.Dispose();
		}
	}
}
