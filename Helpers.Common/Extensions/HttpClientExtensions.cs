using System.Text.Json;

namespace System.Net.Http;

public static class HttpClientExtensions
{

	public async static Task<T> GetAsync<T>(this HttpClient client, Uri requestUri, CancellationToken cancellationToken = default)
	{
		using var response = await client.GetAsync(requestUri, cancellationToken: cancellationToken);

		await using var stream = await response.Content.ReadAsStreamAsync();

		try
		{
			return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken)
				?? throw new ArgumentOutOfRangeException(nameof(stream), stream, "failed to deserialize stream");
		}
		catch (Exception ex)
		{
			ex.Data.Add("type", typeof(T).FullName);

			if (stream.CanSeek)
			{
				stream.Position = 0L;

				using var reader = new StreamReader(stream);

				var json = await reader.ReadToEndAsync();

				ex.Data.Add(nameof(json), json);
			}

			throw;
		}
	}
}
