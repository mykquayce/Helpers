using Helpers.Elgato.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Elgato.Concrete;

public class WhiteLightClient(HttpClient httpClient) : IWhiteLightClient
{
	public Task<Info> GetInfoAsync(CancellationToken cancellationToken = default)
		=> httpClient.GetFromJsonAsync<Info>("elgato/accessory-info", SourceGenerationContext.Default.Info, cancellationToken);

	public async Task<WhiteLight> GetAsync(CancellationToken cancellationToken = default)
	{
		var message = await httpClient.GetFromJsonAsync("elgato/lights", SourceGenerationContext.Default.MessageWhiteLight, cancellationToken);
		return message.lights.First();
	}

	public Task<HttpResponseMessage> SetAsync(WhiteLight light, CancellationToken cancellationToken = default)
	{
		var message = new Message<WhiteLight>(1, [light,]);
		var json = JsonSerializer.Serialize(message, SourceGenerationContext.Default.MessageWhiteLight);
		return httpClient.PutAsync("elgato/lights", new StringContent(json), cancellationToken);
	}
}
