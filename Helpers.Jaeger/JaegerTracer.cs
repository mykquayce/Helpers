using Dawn;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using OpenTracing;
using OpenTracing.Propagation;

namespace Helpers.Jaeger
{
	public class JaegerTracer : ITracer
	{
		private readonly ITracer _tracer;

		public JaegerTracer(Models.Settings settings)
		{
			Guard.Argument(() => settings).NotNull();
			Guard.Argument(() => settings.ServiceName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Host).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Port).NotNull().InRange(1, 65_535);

			var sender = new UdpSender(settings.Host!, settings.Port!.Value, maxPacketSize: 0);

			var reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();

			var sampler = new ConstSampler(sample: true);

			_tracer = new Tracer.Builder(settings.ServiceName!)
				.WithReporter(reporter)
				.WithSampler(sampler)
				.Build();
		}

		#region ITracer implementation
		public IScopeManager ScopeManager => _tracer.ScopeManager;
		public ISpan ActiveSpan => _tracer.ActiveSpan;
		public ISpanBuilder BuildSpan(string operationName) => _tracer.BuildSpan(operationName);
		public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier) => _tracer.Extract(format, carrier);
		public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier) => _tracer.Inject(spanContext, format, carrier);
		#endregion ITracer implementation
	}
}
