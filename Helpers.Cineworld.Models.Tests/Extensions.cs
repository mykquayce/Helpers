using System.Xml.Serialization;

namespace Helpers.Cineworld.Models.Tests;

public static class Extensions
{
	private readonly static XmlSerializerFactory _serializerFactory = new();

	public static T? Deserialize<T>(this Stream stream)
		where T : class
		=> _serializerFactory.CreateSerializer(typeof(T)).Deserialize(stream) as T;
}
