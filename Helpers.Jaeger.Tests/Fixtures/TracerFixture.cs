using Jaeger;
using Jaeger.Samplers;
using OpenTracing;
using System;

namespace Helpers.Jaeger.Tests.Fixtures
{
	public sealed class TracerFixture : IDisposable
	{
		public TracerFixture()
		{
			var settings = new SettingsFixture().Settings;
			var reporter = new ReporterFixture().Reporter;
			var sampler = new ConstSampler(sample: true);

			Tracer = new Tracer.Builder(settings.ServiceName)
				.WithReporter(reporter)
				.WithSampler(sampler)
				.Build();
		}

		public ITracer Tracer { get; }

		public void Dispose() => (Tracer as Tracer)?.Dispose();
	}
}
