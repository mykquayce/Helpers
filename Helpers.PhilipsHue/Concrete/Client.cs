using Dawn;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Helpers.PhilipsHue.Concrete;

public partial class Client : IClient
{
	private readonly string _uriPrefix;
	private readonly HttpClient _httpClient;
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	public Client(IOptions<Config> options, HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;

		_uriPrefix = "/api/" + Guard.Argument(config).Wrap(o => o.Username)
			.NotNull().NotEmpty().NotWhiteSpace().Value;
		_httpClient = httpClient;
		_jsonSerializerOptions = jsonSerializerOptions;
	}

	private async Task<T> GetFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken = default)
	{
		return (await _httpClient.GetFromJsonAsync<T>(requestUri, _jsonSerializerOptions, cancellationToken))
			?? throw new Exceptions.DeserializationException<T>(requestUri);
	}

	private Task PutAsJsonAsync(string requestUri, object body, CancellationToken cancellationToken = default)
	{
		return _httpClient.PutAsJsonAsync(requestUri, body, _jsonSerializerOptions, cancellationToken);
	}
}
