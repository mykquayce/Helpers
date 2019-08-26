using Dawn;
using Helpers.Jaeger.Models;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Microsoft.Extensions.Configuration;
using OpenTracing;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class JaegerConfigurationExtensions
	{
		public static IServiceCollection AddJaegerTracing(this IServiceCollection services, IConfiguration configuration)
		{
			Guard.Argument(() => configuration).NotNull();

			var serviceName = Guard.Argument(() => configuration["serviceName"]).NotNull().NotEmpty().NotWhiteSpace().Value;
			var host = Guard.Argument(() => configuration["host"]).NotNull().NotEmpty().NotWhiteSpace().Value;
			var port = Guard.Argument(() => configuration["port"]).NotNull().NotEmpty().NotWhiteSpace().Matches(@"^\d{1,5}$").Cast<int>().Value;

			return AddJaegerTracing(services, serviceName, host, port);
		}

		public static IServiceCollection AddJaegerTracing(this IServiceCollection services, Settings settings)
		{
			Guard.Argument(() => settings).NotNull();
			Guard.Argument(() => settings.ServiceName).NotNull();

			return AddJaegerTracing(services, settings.ServiceName!, settings.Host, settings.Port);
		}

		public static IServiceCollection AddJaegerTracing(
			this IServiceCollection services,
			string serviceName,
			string? host = "localhost",
			int? port = 6_831)
		{
			Guard.Argument(() => services).NotNull();
			Guard.Argument(() => serviceName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => host).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => port).NotNull().InRange(1, 65_535);

			try
			{
				if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));

				var sender = new UdpSender(host, port!.Value, maxPacketSize: 0);

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
			catch (Exception ex)
			{
				ex.Data.Add(nameof(serviceName), serviceName);
				ex.Data.Add(nameof(host), host);
				ex.Data.Add(nameof(port), port);

				throw;
			}
		}
	}
}
