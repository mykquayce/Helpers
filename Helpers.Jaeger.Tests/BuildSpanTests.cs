using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Jaeger.Thrift.Senders.Internal;
using OpenTracing;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class BuildSpanTests
	{
		private readonly ITracer _tracer;

		public BuildSpanTests()
		{
			var sender = new UdpSender("localhost", 6_831, maxPacketSize: ThriftUdpClientTransport.MaxPacketSize);

			var reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();

			var sampler = new ConstSampler(sample: true);

			_tracer = new Tracer.Builder("testservice2")
				.WithReporter(reporter)
				.WithSampler(sampler)
				.Build();
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
