using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests
{
	public class IPEndPointConverter
	{
		private record Record([property: JsonConverter(typeof(Converters.JsonIPEndPointConverter))] IPEndPoint IPEndPoint);

		[Theory]
		[InlineData(@"{""IPEndPoint"":null}")]
		[InlineData(@"{""IPEndPoint"":""255.255.255.255:9999""}")]
		public void Test(string json)
		{
			var record = JsonSerializer.Deserialize<Record>(json);

			Assert.NotNull(record);

			var jsonAgain = JsonSerializer.Serialize(record);

			Assert.Equal(json, jsonAgain);
		}

		[Theory]
		[InlineData("1", new byte[] { 0, 0, 0, 1, }, 0)]
		[InlineData("20.2:80", new byte[] { 20, 0, 0, 2, }, 80)]
		[InlineData("20.65535:23", new byte[] { 20, 0, 255, 255, }, 23)]
		[InlineData("128.1.2:443", new byte[] { 128, 1, 0, 2, }, 443)]
		[InlineData("1fff:0:a88:85a3::ac1f", new byte[] { 31, 255, 0, 0, 10, 136, 133, 163, 0, 0, 0, 0, 0, 0, 172, 31, }, 0)]
		[InlineData("[1fff:0:a88:85a3::ac1f]", new byte[] { 31, 255, 0, 0, 10, 136, 133, 163, 0, 0, 0, 0, 0, 0, 172, 31, }, 0)]
		[InlineData("[1fff:0:a88:85a3::ac1f]:8001", new byte[] { 31, 255, 0, 0, 10, 136, 133, 163, 0, 0, 0, 0, 0, 0, 172, 31, }, 8_001)]
		[InlineData("::1", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, }, 0)]
		[InlineData("[::1]", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, }, 0)]
		[InlineData("[::1]:2134", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, }, 2_134)]
		[InlineData("255.255.255.255:9999", new byte[] { 255, 255, 255, 255, }, 9_999)]
		public void ParseTests(string s, byte[] expectedBytes, ushort expectedPort)
		{
			var ok = Converters.JsonIPEndPointConverter.TryParse(s, out var actual);

			Assert.True(ok);
			Assert.Equal(expectedBytes, actual.Address.GetAddressBytes());
			Assert.Equal(expectedPort, actual.Port);
		}
	}
}
