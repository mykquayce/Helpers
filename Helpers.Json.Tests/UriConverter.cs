using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests
{
	public class UriConverter
	{
		private record Record([property: JsonConverter(typeof(Converters.JsonUriConverter))] Uri Uri);

		[Theory]
		[InlineData(@"{""Uri"":""https://old.reddit.com/""}")]
		[InlineData(@"{""Uri"":""https://old.reddit.com:2342/""}")]
		[InlineData(@"{""Uri"":""https://old.reddit.com/r/random""}")]
		[InlineData(@"{""Uri"":""/r/random""}")]
		[InlineData(@"{""Uri"":""/r/random?key1=value""}")]
		[InlineData(@"{""Uri"":""/r/random?key1=value\u0026key2=value""}")]
		public void Test(string json)
		{
			var record = JsonSerializer.Deserialize<Record>(json);

			Assert.NotNull(record);
			Assert.NotNull(record!.Uri);

			var jsonAgain = JsonSerializer.Serialize(record);

			Assert.Equal(json, jsonAgain);
		}
	}
}
