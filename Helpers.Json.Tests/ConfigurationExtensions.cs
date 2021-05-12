using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Xunit;

namespace Helpers.Json.Tests
{
	public class ConfigurationExtensions
	{
		[Theory]
		[InlineData(@"{""DayOfWeek"":""Tuesday"",""Number"":1,""Boolean"":true}", DayOfWeek.Tuesday, 1, true)]
		[InlineData(@"{""DayOfWeek"":""Sunday"",""Number"":0,""Boolean"":false}", DayOfWeek.Sunday, 0, false)]
		public void GetTypeTests(string json, DayOfWeek expectedDayOfWeek, int expectedNumber, bool expectedBoolean)
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			var stream = new MemoryStream(bytes);

			var configuration = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			var response = configuration.JsonConfig<Response>();

			Assert.NotNull(response);
			Assert.Equal(expectedDayOfWeek, response!.DayOfWeek);
			Assert.Equal(expectedNumber, response.Number);
			Assert.Equal(expectedBoolean, response.Boolean);
		}

		[Theory]
		[InlineData("string", "string")]
		[InlineData("1", 1)]
		[InlineData("2", 2)]
		[InlineData("true", true)]
		[InlineData("True", true)]
		public void StringToObjectTests(string s, object expected)
		{
			var actual = s.ToObject();
			Assert.Equal(expected, actual);
		}
	}

	public record Response(
		[property: JsonConverter(typeof(JsonStringEnumConverter))] DayOfWeek DayOfWeek,
		int Number,
		bool Boolean);
}
