using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.IO;

public static class StreamExtensions
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		AllowTrailingCommas = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
	};

	public static Task<T?> DeserializeAsync<T>(this Stream stream, CancellationToken? cancellationToken = default)
		=> JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None).AsTask();

	public static string? ReadAsString(this Stream stream)
	{
		if (!stream.CanRead)
		{
			return null;
		}

		if (stream.Position > 0)
		{
			if (!stream.CanSeek)
			{
				return null;
			}

			stream.Position = 0;
		}

		var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}
