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
		public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPEndPoint endPoint)
		{
			var uri = new UriBuilder("http", endPoint.Address.ToString(), endPoint.Port, "/elgato/accessory-info").Uri;

			var response = await base.SendAsync<Models.AccessoryInfoObject>(HttpMethod.Get, uri);

			return response.Object ?? throw new Exception();
		}

		public async Task<Models.MessageObject.LightObject> GetLightAsync(IPEndPoint endPoint)
		{
			var uri = new UriBuilder("http", endPoint.Address.ToString(), endPoint.Port, "/elgato/lights").Uri;

			var response = await base.SendAsync<Models.MessageObject>(HttpMethod.Get, uri);

			return response.Object?.lights?.FirstOrDefault()
				?? throw new Exception();
		}

		public Task SetLightAsync(IPEndPoint endPoint, Models.MessageObject.LightObject light)
		{
			var uri = new UriBuilder("http", endPoint.Address.ToString(), endPoint.Port, "/elgato/lights").Uri;

			var messageObject = new Models.MessageObject(numberOfLights: 1, lights: new Models.MessageObject.LightObject[1] { light, });

			var json = JsonSerializer.Serialize(messageObject);

			return base.SendAsync<Models.MessageObject>(HttpMethod.Put, uri, json);
		}
	}
}
