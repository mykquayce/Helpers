using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using OpenTracing;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class JaegerConfigurationExtensions
	{
		public static IServiceCollection AddJaegerTracing(this IServiceCollection services, string serviceName, string host = "localhost", int port = 6831)
		{
			if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));

			var sender = new UdpSender(host, port, maxPacketSize: 0);

			var reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();

			var sampler = new ConstSampler(sample: true);

			var tracer = new Tracer.Builder(serviceName)
				.WithReporter(reporter)
				.WithSampler(sampler)
				.Build();

			services
				.AddOpenTracing()
				.AddSingleton<ITracer>(tracer);

			return services;
		}
	}
}
