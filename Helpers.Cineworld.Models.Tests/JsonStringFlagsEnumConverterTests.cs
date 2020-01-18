using System;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class JsonStringFlagsEnumConverterTests
	{
		[Theory]
		[InlineData("hello world")]
		public void JsonStringFlagsEnumConverterTests_Utf8JsonReader_BehavesPredictably(string message)
		{
			var bytes = Encoding.UTF8.GetBytes(@$"{{ ""message"": ""{message}"" }}");
			var sut = new Utf8JsonReader(bytes, isFinalBlock: true, new JsonReaderState());

			sut.WalkToFirstValue();

			Assert.True(sut.ValueTextEquals(message));

			var actual = sut.GetString();

			Assert.Equal(message, actual);
		}

		[Theory]
		[InlineData("One", FlagsEnums.One)]
		[InlineData("Two", FlagsEnums.Two)]
		[InlineData("Three", FlagsEnums.Three)]
		[InlineData("One|Two", FlagsEnums.Three)]
		[InlineData("Four", FlagsEnums.Four)]
		[InlineData("Five", FlagsEnums.Five)]
		public void JsonStringFlagsEnumConverterTests_Read_BehavesPredictably(string message, FlagsEnums expected)
		{
			var sut = new JsonStringFlagsEnumConverter<FlagsEnums>();

			var s = $@"{{ ""message"": ""{message}"" }}";
			var bytes = Encoding.UTF8.GetBytes(s);
			var reader = new Utf8JsonReader(bytes);

			reader.WalkToFirstValue();

			Assert.True(reader.ValueTextEquals(message));

			var actual = sut.Read(ref reader, typeToConvert: default, options: default);

			Assert.Equal(expected, actual);
		}

		[Flags]
		public enum FlagsEnums : byte
		{
			None = 0,
			One = 1,
			Two = 2,
			Three = One | Two,
			Four = 4,
			Five = One | Four,
			Six = Two | Four,
			Seven = One | Two | Four,
			Eight = 8,
		}
	}
}
