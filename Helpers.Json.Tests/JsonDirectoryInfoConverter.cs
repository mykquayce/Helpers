using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests;

public class JsonDirectoryInfoConverter
{
	private record Record([property: JsonConverter(typeof(Converters.JsonDirectoryInfoConverter))] DirectoryInfo DirectoryInfo);

	[Theory]
	[InlineData(@"{""DirectoryInfo"":""./Data""}")]
	public void Test(string json)
	{
		var record = JsonSerializer.Deserialize<Record>(json);

		Assert.NotNull(record);
		Assert.NotNull(record!.DirectoryInfo);
		Assert.True(record.DirectoryInfo.Exists);
	}
}
