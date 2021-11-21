using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;

namespace Helpers.Json.Tests
{
	public class Deserialization
	{
		[Theory]
		[InlineData(@"{""IPAddress"":""127.0.0.1"",""PhysicalAddress"":""a03ccf40b1c9""}", "127.0.0.1", "a03ccf40b1c9")]
		[InlineData(@"{""IPAddress"":""127.0.0.2"",""PhysicalAddress"":""e7387c1012c5""}", "127.0.0.2", "e7387c1012c5")]
		[InlineData(@"{""IPAddress"":""::1"",""PhysicalAddress"":""a2c9e9444075""}", "::1", "a2c9e9444075")]
		[InlineData(@"{""IPAddress"":""2a00:1450:4009:80a::2004"",""PhysicalAddress"":""47909a2f4227""}", "2a00:1450:4009:80a::2004", "47909a2f4227")]
		[InlineData(@"{""IPAddress"":""49aa:7252:307f:477:6772:2549:1c1:bec6"",""PhysicalAddress"":""0d3474e17624""}", "49aa:7252:307f:477:6772:2549:1c1:bec6", "0d3474e17624")]
		public void JsonConverterTest(string json, string expectedIPAddress, string expectedPhysicalAddress)
		{
			var config = JsonSerializer.Deserialize<Addresses>(json);

			Assert.NotNull(config);
			Assert.NotNull(config!.IPAddress);
			Assert.Equal(expectedIPAddress, config.IPAddress.ToString());
			Assert.NotNull(config!.PhysicalAddress);
			Assert.Equal(expectedPhysicalAddress, config.PhysicalAddress.ToString().ToLowerInvariant());

			Assert.Equal(json, JsonSerializer.Serialize(config));
		}

		[Theory]
		[InlineData(@"{""0"":""first"",""1"":""second""}", "first", "second")]
		[InlineData(@"{""1"":""first"",""2"":""second""}", "first", "second")]
		[InlineData(@"{""-821936492"":""first"",""-3483"":""second"",""6498273"":""third""}", "first", "second", "third")]
		public void ConfigurationArrayTests(string json, params string[] expected)
		{
			IServiceProvider serviceProvider;
			{
				IConfiguration configuration;
				{
					var bytes = System.Text.Encoding.UTF8.GetBytes(json);
					using var stream = new MemoryStream(bytes);
					configuration = new ConfigurationBuilder()
						.AddJsonStream(stream)
						.Build();
				}

				serviceProvider = new ServiceCollection()
					.Configure<List<string>>(configuration)
					.BuildServiceProvider();
			}

			var options = serviceProvider.GetService<IOptions<List<string>>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var actual = options.Value;

			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.DoesNotContain(default, actual);
			Assert.Equal(expected, actual);
		}
	}
}
