using System.Text.Json;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public static class ExtensionMethods
	{
		public static void WalkToFirstValue(this ref Utf8JsonReader reader)
		{
			Assert.Equal(JsonTokenType.None, reader.TokenType);
			reader.Read();
			Assert.Equal(JsonTokenType.StartObject, reader.TokenType);
			reader.Read();
			Assert.Equal(JsonTokenType.PropertyName, reader.TokenType);
			reader.Read();
			Assert.Equal(JsonTokenType.String, reader.TokenType);
		}
	}
}
