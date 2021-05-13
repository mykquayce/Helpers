using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests
{
	public class BigIntegerConverter
	{
		private record Record([property: JsonConverter(typeof(Converters.JsonBigIntegerConverter))] BigInteger BigInteger);

		[Theory]
		[InlineData(@"{""BigInteger"":""-234702348570928347509283475023450987092834""}")]
		[InlineData(@"{""BigInteger"":""-1""}")]
		[InlineData(@"{""BigInteger"":""1""}")]
		[InlineData(@"{""BigInteger"":""234702348570928347509283475023450987092834""}")]
		public void Test(string json)
		{
			var record = JsonSerializer.Deserialize<Record>(json);

			Assert.NotNull(record);
			Assert.NotEqual(0, record!.BigInteger);

			var jsonAgain = JsonSerializer.Serialize(record);

			Assert.Equal(json, jsonAgain);
		}
	}
}
