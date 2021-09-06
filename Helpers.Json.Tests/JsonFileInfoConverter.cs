using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests;

public class JsonFileInfoConverter
{
	private record Record([property: JsonConverter(typeof(Converters.JsonFileInfoConverter))] FileInfo FileInfo);

	[Theory]
	[InlineData(@"{""FileInfo"":""./Data/message.txt""}")]
	public void Test(string json)
	{
		var record = JsonSerializer.Deserialize<Record>(json);

		Assert.NotNull(record);
		Assert.NotNull(record!.FileInfo);
		Assert.True(record.FileInfo.Exists);
	}
}
