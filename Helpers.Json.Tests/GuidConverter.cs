using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests
{
	public class GuidConverter
	{
		private record Record(
			Guid? WithoutConverter,
			[property: JsonConverter(typeof(Converters.JsonGuidConverter))] Guid WithConverter);

		[Theory]
		[InlineData(@"{""WithConverter"":""f31254bf-7b67-5e3a-6766-af5069453450""}", "f31254bf-7b67-5e3a-6766-af5069453450")]
		[InlineData(@"{""WithConverter"":""F31254BF-7B67-5E3A-6766-AF5069453450""}", "f31254bf-7b67-5e3a-6766-af5069453450")]
		[InlineData(@"{""WithConverter"":""f31254bf7b675e3a6766af5069453450""}", "f31254bf-7b67-5e3a-6766-af5069453450")]
		[InlineData(@"{""WithConverter"":""F31254BF7B675E3A6766AF5069453450""}", "f31254bf-7b67-5e3a-6766-af5069453450")]
		public void Test(string json, string expected)
		{
			var record = JsonSerializer.Deserialize<Record>(json);

			Assert.NotNull(record);
			Assert.NotEqual(default, record!.WithConverter);
			Assert.Equal(Guid.Parse(expected), record.WithConverter);
		}

		[Theory]
		[InlineData(@"{""WithoutConverter"":""f31254bf7b675e3a6766af5069453450""}")]
		[InlineData(@"{""WithoutConverter"":""F31254BF7B675E3A6766AF5069453450""}")]
		public void ProveNecessity(string json)
		{
			void action() => JsonSerializer.Deserialize<Record>(json);
			Assert.Throws<JsonException>(action);
		}
	}
}
