using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.Elgato.Concrete
{
	public class ElgatoClient : Helpers.Web.WebClientBase, IElgatoClient
	{
		public record Config(string Scheme, int Port)
		{
			public const string DefaultScheme = "http";
			public const int DefaultPort = 9_123;

			public static Config Defaults => new(DefaultScheme, DefaultPort);
		}

		private readonly Config _config;

		public ElgatoClient() : this(Config.Defaults) { }
		public ElgatoClient(IOptions<Config> options) : this(options.Value) { }

		public ElgatoClient(Config config)
		{
			_config = Guard.Argument(() => config).NotNull().Value;
		}

		public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress)
		{
			Guard.Argument(() => ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
			var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
			var uri = new Uri(baseUri, "/elgato/accessory-info");
			var response = await base.SendAsync<Models.AccessoryInfoObject>(HttpMethod.Get, uri);
			return response.Object ?? throw new Exception();
		}

		public async Task<Models.MessageObject.LightObject> GetLightAsync(IPAddress ipAddress)
		{
			Guard.Argument(() => ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
			var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
			var uri = new Uri(baseUri, "/elgato/lights");
			var response = await base.SendAsync<Models.MessageObject>(HttpMethod.Get, uri);
			return response.Object?.lights?.FirstOrDefault()
				?? throw new Exception();
		}

		public Task SetLightAsync(IPAddress ipAddress, Models.MessageObject.LightObject light)
		{
			Guard.Argument(() => ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
			var baseUri = new UriBuilder(_config.Scheme, ipAddress.ToString(), _config.Port).Uri;
			var uri = new Uri(baseUri, "/elgato/lights");
			var messageObject = new Models.MessageObject(numberOfLights: 1, lights: new Models.MessageObject.LightObject[1] { light, });
			var json = JsonSerializer.Serialize(messageObject);
			return base.SendAsync<Models.MessageObject>(HttpMethod.Put, uri, json);
		}

		public async Task ToggleLightAsync(IPAddress ipAddress)
		{
			Guard.Argument(() => ipAddress).NotNull().Wrap(ip => ip.GetAddressBytes()).NotEmpty();
			var before = await GetLightAsync(ipAddress);
			var after = before with { on = before.on == 1 ? (byte)0 : (byte)1, };
			await SetLightAsync(ipAddress, after);
		}
	}
}
