using Microsoft.Extensions.DependencyInjection;
using System;

namespace Helpers.Jaeger.Tests.Fixtures
{
	public sealed class ServiceProviderFixture : IDisposable
	{
		public ServiceProviderFixture()
		{
			var settings = new SettingsFixture().Settings;

			ServiceProvider = new ServiceCollection()
				.AddJaegerTracing(settings)
				.BuildServiceProvider();
		}

		public IServiceProvider ServiceProvider { get; }
		public void Dispose() => (ServiceProvider as ServiceProvider)?.Dispose();
	}
}
