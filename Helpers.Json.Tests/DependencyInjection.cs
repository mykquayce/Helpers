using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.Json.Tests
{
	public class DependencyInjection
	{
		private readonly IConfiguration _configuration;

		public DependencyInjection()
		{
			IEnumerable<KeyValuePair<string, string>> values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["Addresses:IPAddress"] = "127.0.0.1",
				["Addresses:PhysicalAddress"] = "d346f2d525a9",
			};

			_configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(values)
				.Build();
		}

		[Fact]
		public void Configuration()
		{
			IConfiguration config = _configuration.GetSection(nameof(Addresses));
			var addresses = config.JsonConfig<Addresses>();

			Assert.NotNull(addresses);
			Assert.NotNull(addresses!.IPAddress);
			Assert.NotEqual(IPAddress.None, addresses.IPAddress);
			Assert.NotNull(addresses.PhysicalAddress);
			Assert.NotEqual(PhysicalAddress.None, addresses.PhysicalAddress);
		}

		[Fact]
		public void ServiceCollection()
		{
			var config = _configuration.GetSection(nameof(Addresses));

			IServiceProvider serviceProvider = new ServiceCollection()
				.JsonConfig<Addresses>(config)
				.BuildServiceProvider();

			var options = serviceProvider.GetService<IOptions<Addresses>>();

			Assert.NotNull(options);

			var addresses = options!.Value;

			Assert.NotNull(addresses);
			Assert.NotNull(addresses!.IPAddress);
			Assert.NotEqual(IPAddress.None, addresses.IPAddress);
			Assert.NotNull(addresses.PhysicalAddress);
			Assert.NotEqual(PhysicalAddress.None, addresses.PhysicalAddress);
		}
	}
}
