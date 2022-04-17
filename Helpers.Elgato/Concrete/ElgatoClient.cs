using Dawn;
using System.Text.Json;

namespace Helpers.Elgato.Concrete;

public class ElgatoClient : Helpers.Web.WebClientBase, IElgatoClient
{
	public ElgatoClient(HttpClient httpClient)
		: base(httpClient)
	{ }

	public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/elgato/accessory-info", UriKind.Relative);
		var (_, _, info) = await base.SendAsync<Models.AccessoryInfoObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return info ?? throw new Exception();
	}

	public async IAsyncEnumerable<Models.MessageObject.LightObject> GetLightAsync(CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/elgato/lights", UriKind.Relative);
		var (_, _, message) = await base.SendAsync<Models.MessageObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		foreach (var light in message.lights) yield return light;
	}

	public Task SetLightAsync(Models.MessageObject.LightObject light, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(light).NotNull();
		var uri = new Uri("/elgato/lights", UriKind.Relative);
		var messageObject = new Models.MessageObject(numberOfLights: 1, lights: new Models.MessageObject.LightObject[1] { light, });
		var json = JsonSerializer.Serialize(messageObject);
		return base.SendAsync<Models.MessageObject>(HttpMethod.Put, uri, json, cancellationToken: cancellationToken);
	}
}
