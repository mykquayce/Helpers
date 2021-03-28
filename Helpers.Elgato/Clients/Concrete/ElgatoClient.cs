using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.Elgato.Clients.Concrete
{
	public class ElgatoClient : Helpers.Web.WebClientBase, IElgatoClient
	{
		public record Config(
			string Scheme = Config.DefaultScheme,
			string Host = Config.DefaultHost,
			ushort Port = Config.DefaultPort)
		{
			public const string DefaultScheme = "http";
			public const string DefaultHost = "localhost";
			public const ushort DefaultPort = 9_123;

			public Config() : this(DefaultScheme, DefaultHost, DefaultPort) { }

			public Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;

			public override string ToString() => Uri.ToString();
		}

		public ElgatoClient(HttpClient httpClient, ILogger? logger = default, ITracer? tracer = default)
			: base(httpClient, logger, tracer)
		{ }

		public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync()
		{
			var uri = new Uri("/elgato/accessory-info", UriKind.Relative);

			var response = await base.SendAsync<Models.AccessoryInfoObject>(HttpMethod.Get, uri);

			return response.Object ?? throw new Exception();
		}

		public async Task<Models.MessageObject.LightObject> GetLightAsync()
		{
			var uri = new Uri("/elgato/lights", UriKind.Relative);

			var response = await base.SendAsync<Models.MessageObject>(HttpMethod.Get, uri);

			return response.Object?.lights?.FirstOrDefault()
				?? throw new Exception();
		}

		public Task SetLightAsync(Models.MessageObject.LightObject light)
		{
			var uri = new Uri("/elgato/lights", UriKind.Relative);

			var messageObject = new Models.MessageObject(numberOfLights: 1, lights: new Models.MessageObject.LightObject[1] { light, });

			var json = JsonSerializer.Serialize(messageObject);

			return base.SendAsync<Models.MessageObject>(HttpMethod.Put, uri, json);
		}
	}
}
