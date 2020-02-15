using Dawn;
using Helpers.Jaeger.Models;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using OpenTracing;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class JaegerConfigurationExtensions
	{
		public static IServiceCollection AddJaegerTracing(
			this IServiceCollection services,
			string serviceName,
			string? host = default,
			int? port = default)
		{
			var settings = new Settings
			{
				ServiceName = serviceName,
				Host = host ?? Settings.DefaultHost,
				Port = port ?? Settings.DefaultPort,
			};

			return AddJaegerTracing(services, settings);
		}

		public static IServiceCollection AddJaegerTracing(
			this IServiceCollection services,
			Settings settings)
		{
			Guard.Argument(() => settings).NotNull();
			Guard.Argument(() => settings.ServiceName!).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Host).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => settings.Port).InRange(1, 65_535);

			var sender = new UdpSender(settings.Host, settings.Port, maxPacketSize: 0);

			var reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();

			var sampler = new ConstSampler(sample: true);

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
