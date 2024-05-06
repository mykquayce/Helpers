using System.Xml.Serialization;

namespace System.Net.Http;

public static class HttpClientExtensions
{
	private static readonly XmlSerializerFactory _xmlSerializerFactory = new();

	public static async Task<T> GetFromXml<T>(this HttpClient httpClient, Uri requestUri, CancellationToken cancellationToken = default)
	{
		var stream = await httpClient.GetStreamAsync(requestUri, cancellationToken);
		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
		return (T)serializer.Deserialize(stream)!;
	}
}
