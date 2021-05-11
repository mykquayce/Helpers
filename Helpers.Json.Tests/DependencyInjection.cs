using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Helpers.Json.Tests
{
	public class DependencyInjection
	{
		[Fact]
		public void Configuration()
		{
			IEnumerable<KeyValuePair<string, string>> values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["Addresses:IPAddress"] = "127.0.0.1",
				["Addresses:PhysicalAddress"] = "d346f2d525a9",
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(values)
				.Build();

			var addresses = configuration.GetSection("Addresses").GetType<Addresses>();

			Assert.NotNull(addresses);
			Assert.NotNull(addresses!.IPAddress);
			Assert.Equal("127.0.0.1", addresses.IPAddress.ToString().ToLowerInvariant());
			Assert.NotNull(addresses!.PhysicalAddress);
			Assert.Equal("d346f2d525a9", addresses.PhysicalAddress.ToString().ToLowerInvariant());
		}
	}
}
