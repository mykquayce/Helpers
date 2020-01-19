using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public static class ExtensionMethods
	{
		private static readonly XmlSerializerFactory _xmlSerializerFactory = new XmlSerializerFactory();

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

		public static T DeserializeFile<T>(this string fileName)
		{
			var path = Path.Combine("Data", fileName);

			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

			return (T)serializer.Deserialize(stream);
		}
	}
}
