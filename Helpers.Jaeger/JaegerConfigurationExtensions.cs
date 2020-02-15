using Dawn;
using Microsoft.Extensions.Configuration;
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
			Helpers.Jaeger.Models.Settings settings)
		{
			Guard.Argument(() => settings).NotNull();

			var tracer = new Helpers.Jaeger.JaegerTracer(settings);

			return services
				.AddOpenTracing()
				.AddSingleton<ITracer>(tracer);
		}
	}
}
