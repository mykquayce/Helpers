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
		[InlineData(@"{""IPEndPoint"":""255.255.255.255:9999""}")]
		public void Test(string json)
		{
			var record = JsonSerializer.Deserialize<Record>(json);

			Assert.NotNull(record);
			Assert.NotNull(record!.IPEndPoint);

			var jsonAgain = JsonSerializer.Serialize(record);

			Assert.Equal(json, jsonAgain);
		}
	}
}
