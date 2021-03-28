using System.Threading.Tasks;

namespace Helpers.Elgato.Services.Concrete
{
	public sealed class ElgatoService : IElgatoService
	{
		private readonly Clients.IElgatoClient _client;

		public ElgatoService(Clients.IElgatoClient client)
		{
			_client = client;
		}

		public void Dispose() => _client?.Dispose();

		public Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync() => _client.GetAccessoryInfoAsync();

		public Task<Models.MessageObject.LightObject> GetLightAsync() => _client.GetLightAsync();

		public Task SetLightAsync(Models.MessageObject.LightObject light) => _client.SetLightAsync(light);

		public async Task ToggleLightPowerStateAsync()
		{
			var old = await _client.GetLightAsync();

			var @new = old with { on = old.on == 1 ? (byte)0 : (byte)1, };

			await _client.SetLightAsync(@new);
		}
	}
}
