using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace Helpers.Json.Tests
{
	public class DependencyInjection
	{
		private readonly IConfiguration _configuration;

		public DependencyInjection()
		{
			IEnumerable<KeyValuePair<string, string?>> values = new Dictionary<string, string?>(StringComparer.InvariantCultureIgnoreCase)
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

			var provider = new ServiceCollection()
				.JsonConfig<Addresses>(config)
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<Addresses>>();

			Assert.NotNull(options);

			var addresses = options!.Value;

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

		[Theory]
		[InlineData(@"{""DayOfWeek"":""Tuesday"",""Number"":1,""Boolean"":true}", DayOfWeek.Tuesday, 1, true)]
		[InlineData(@"{""DayOfWeek"":""Sunday"",""Number"":0,""Boolean"":false}", DayOfWeek.Sunday, 0, false)]
		public void GetTypeTests(string json, DayOfWeek expectedDayOfWeek, int expectedNumber, bool expectedBoolean)
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			using var stream = new MemoryStream(bytes);

			var configuration = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			var provider = new ServiceCollection()
				.JsonConfig<Response>(configuration)
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<Response>>();

			Assert.NotNull(options);

			var response = options!.Value;

			Assert.NotNull(response);
			Assert.Equal(expectedDayOfWeek, response!.DayOfWeek);
			Assert.Equal(expectedNumber, response.Number);
			Assert.Equal(expectedBoolean, response.Boolean);
		}

		private record Response(DayOfWeek DayOfWeek, int Number, bool Boolean);

		[Fact]
		public void Test1_Enum_PhysicalAddress()
		{
			var before = new Dictionary<string, string?>
			{
				["AmpStartPlug"] = "003192e1a474",
				["IRBlaster"] = "000c1e059cad",
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(before)
				.Build();

			var provider = new ServiceCollection()
				.JsonConfig<IDictionary<Devices, PhysicalAddress>>(configuration)
				.BuildServiceProvider();

			var after = provider.GetService<IOptions<IDictionary<Devices, PhysicalAddress>>>();

			Assert.NotNull(after);
			Assert.NotNull(after!.Value);
			Assert.NotEmpty(after.Value);
		}

		[Fact]
		public void Test1_Enum_Int()
		{
			var before = new Dictionary<string, string?>
			{
				["AmpStartPlug"] = "1",
				["IRBlaster"] = "1",
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(before)
				.Build();

			var provider = new ServiceCollection()
				.JsonConfig<IDictionary<Devices, int>>(configuration)
				.BuildServiceProvider();

			var after = provider.GetService<IOptions<IDictionary<Devices, int>>>();

			Assert.NotNull(after);
			Assert.NotNull(after!.Value);
			Assert.NotEmpty(after.Value);
		}

		[Flags]
		private enum Devices : byte
		{
			None = 0,
			AmpStartPlug = 1,
			IRBlaster = 2,
		}
	}
}
