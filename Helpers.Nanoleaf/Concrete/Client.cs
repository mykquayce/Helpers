using Helpers.Nanoleaf.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Helpers.Nanoleaf.Concrete;

public class Client(HttpClient httpClient, IOptions<Client.IConfig> config) : IClient
{
	public interface IConfig
	{
		string Token { get; set; }
		Uri BaseAddress { get; set; }
	}

	private readonly string _token = config?.Value?.Token ?? throw new ArgumentNullException(nameof(config));

	public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsync("api/v1/new", content: null, cancellationToken);
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
		return content.auth_token;
	}

	public Task<InfoResponse> GetInfoAsync(CancellationToken cancellationToken = default)
		=> httpClient.GetFromJsonAsync<InfoResponse>("api/v1/" + _token, cancellationToken);

	public Task<HttpResponseMessage> SetEffectAsync(string effect, CancellationToken cancellationToken = default)
	{
		var body = new { select = effect, };
		return httpClient.PutAsJsonAsync($"api/v1/{_token}/effects", body, cancellationToken);
	}

	public Task<HttpResponseMessage> SetOnAsync(bool value, CancellationToken cancellationToken = default)
	{
		var body = new { on = new BooleanValue(value), };
		return httpClient.PutAsJsonAsync($"api/v1/{_token}/state", body, cancellationToken);
	}
}
