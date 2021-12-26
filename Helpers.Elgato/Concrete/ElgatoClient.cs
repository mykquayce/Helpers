using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Helpers.Elgato.Concrete;

public class ElgatoClient : Helpers.Web.WebClientBase, IElgatoClient
{
	#region config
	public record Config(string Scheme, int Port)
		: IOptions<Config>
	{
		public const string DefaultScheme = "http";
		public const int DefaultPort = 9_123;

		public Config() : this(DefaultScheme, DefaultPort) { }

		public static Config Defaults => new();

		#region ioptions implementation
		public Config Value => this;
		#endregion ioptions implementation
	}
	#endregion config

	private readonly Config _config;

	public ElgatoClient(IOptions<Config> options)
	{
		_config = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
	}

	public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
		var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
		var uri = new Uri(baseUri, "/elgato/accessory-info");
		var (_, _, info) = await base.SendAsync<Models.AccessoryInfoObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return info ?? throw new Exception();
	}

	public async IAsyncEnumerable<Models.MessageObject.LightObject> GetLightAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
		var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
		var uri = new Uri(baseUri, "/elgato/lights");
		var (_, _, message) = await base.SendAsync<Models.MessageObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		foreach (var light in message.lights) yield return light;
	}

	public Task SetLightAsync(IPAddress ipAddress, Models.MessageObject.LightObject light, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
		var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
		var uri = new Uri(baseUri, "/elgato/lights");
		var messageObject = new Models.MessageObject(numberOfLights: 1, lights: new Models.MessageObject.LightObject[1] { light, });
		var json = JsonSerializer.Serialize(messageObject);
		return base.SendAsync<Models.MessageObject>(HttpMethod.Put, uri, json, cancellationToken: cancellationToken);
	}
}
