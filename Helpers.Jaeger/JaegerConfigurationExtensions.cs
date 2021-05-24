﻿using Dawn;
using Helpers.Jaeger.Models;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using OpenTracing;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class JaegerConfigurationExtensions
	{
		public static IServiceCollection AddJaegerTracing(
			this IServiceCollection services,
			string serviceName,
			string host = Settings.DefaultHost,
			ushort port = Settings.DefaultPort,
			double samplingRate = Settings.DefaultSamplingRate)
		{
			var settings = new Settings(serviceName, host, port, samplingRate);

			return AddJaegerTracing(services, settings);
		}

		public static IServiceCollection AddJaegerTracing(
			this IServiceCollection services,
			Settings settings)
		{
			Guard.Argument(() => settings).NotNull();
			Guard.Argument(() => settings.ServiceName!).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Host).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.SamplingRate).InRange(0, double.MaxValue);

			var sender = new UdpSender(settings.Host, settings.Port, maxPacketSize: 0);

			var reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();

			ISampler sampler = settings.SamplingRate switch
			{
				double d when d >= 1 => new ConstSampler(sample: true),
				double d when d <= 0 => new ConstSampler(sample: false),
				_ => new ProbabilisticSampler(samplingRate: settings.SamplingRate),
			};

			var tracer = new Tracer.Builder(settings.ServiceName!)
				.WithReporter(reporter)
				.WithSampler(sampler)
				.Build();

			return services
				.AddOpenTracing()
				.AddSingleton<ITracer>(tracer);
		}
	}
}
