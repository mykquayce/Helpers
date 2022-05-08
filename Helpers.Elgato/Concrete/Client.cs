using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Helpers.Elgato.Concrete;

public class Client : Helpers.Web.WebClientBase, IClient
{
	private readonly string _scheme;
	private readonly int _port;

	public Client(IOptions<Config> options, HttpClient httpClient)
		: base(httpClient)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).Value;
		_scheme = Guard.Argument(config.Scheme).NotNull().NotEmpty().NotWhiteSpace().In("http", "https").Value;
		_port = Guard.Argument(config.Port).InRange(1, ushort.MaxValue).Value;
	}

	private Uri BuildUri(IPAddress ipAddress) => new UriBuilder(_scheme, ipAddress.ToString(), _port).Uri;

	public  async Task<Models.Generated.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		var uri = new Uri(BuildUri(ipAddress), "/elgato/accessory-info");
		var (_, _, info) = await base.SendAsync<Models.Generated.AccessoryInfoObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return info ?? throw new Exception();
	}

	public async Task<Models.Generated.LightObject> GetLightAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		var uri = new Uri(BuildUri(ipAddress), "/elgato/lights");
		var (_, _, message) = await base.SendAsync<Models.Generated.MessageObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return message.lights.First();
	}

	public Task SetLightAsync(IPAddress ipAddress, Models.Generated.LightObject light, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		Guard.Argument(light).NotNull();
		var uri = new Uri(BuildUri(ipAddress), "/elgato/lights");
		var messageObject = new Models.Generated.MessageObject(numberOfLights: 1, lights: new[] { light, });
		var json = JsonSerializer.Serialize(messageObject);
		return base.SendAsync<Models.Generated.MessageObject>(HttpMethod.Put, uri, json, cancellationToken: cancellationToken);
	}
}
