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

	private Uri BuildBaseAddress(IPAddress ipAddress) => new UriBuilder(_scheme, ipAddress.ToString(), _port).Uri;

	public  async Task<Models.Generated.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		var baseAddress = BuildBaseAddress(ipAddress);
		var uri = new Uri(baseAddress, "/elgato/accessory-info");
		var (_, _, info) = await base.SendAsync<Models.Generated.AccessoryInfoObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return info ?? throw new Exception();
	}

	public async IAsyncEnumerable<Models.Generated.LightObject> GetLightsAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		var baseAddress = BuildBaseAddress(ipAddress);
		var uri = new Uri(baseAddress, "/elgato/lights");
		var response = await base.SendAsync<Models.Generated.MessageObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		if (response.Exception is not null) throw response.Exception;
		var message = response.Object!;
		foreach (var light in message.lights)
		{
			yield return light;
		}
	}

	public async Task SetLightAsync(IPAddress ipAddress, IReadOnlyCollection<Models.Generated.LightObject> lights, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().NotEqual(IPAddress.None);
		Guard.Argument(lights).NotNull();
		var baseAddress = BuildBaseAddress(ipAddress);
		var uri = new Uri(baseAddress, "/elgato/lights");
		var messageObject = new Models.Generated.MessageObject(numberOfLights: lights.Count, lights: lights);
		var json = JsonSerializer.Serialize(messageObject);
		await base.SendAsync<Models.Generated.MessageObject>(HttpMethod.Put, uri, json, cancellationToken: cancellationToken);
	}
}
