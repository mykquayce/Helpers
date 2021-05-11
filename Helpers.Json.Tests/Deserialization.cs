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
		}
	}
}
