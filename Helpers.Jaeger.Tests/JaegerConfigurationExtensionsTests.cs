using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class JaegerConfigurationExtensionsTests
	{
		[Theory]
		[InlineData("test", "localhost", "6831")]
		public void JaegerConfigurationExtensions_AddJaegerTracing(string serviceName, string host, string portString)
		{
			var configurationMock = new Mock<IConfiguration>();

			configurationMock.Setup(c => c["serviceName"]).Returns(serviceName);
			configurationMock.Setup(c => c["host"]).Returns(host);
			configurationMock.Setup(c => c["port"]).Returns(portString);

			var configuration = configurationMock.Object;

			var services = new ServiceCollection(); //Mock.Of<IServiceCollection>(MockBehavior.Loose);

			services.AddJaegerTracing(configuration);

			global::Jaeger.Tracer? tracer = default;

			foreach (ServiceDescriptor service in services)
			{
				if (service.ImplementationInstance is global::Jaeger.Tracer value)
				{
					tracer = value;
					break;
				}
			}

			Assert.Equal(serviceName, tracer?.ServiceName);
		}

		private class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
		{
			public bool IsReadOnly => false;
			public new IEnumerator<ServiceDescriptor> GetEnumerator() => base.GetEnumerator();
		}
	}
}
