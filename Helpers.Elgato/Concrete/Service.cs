using Dawn;
using System.Drawing;
using System.Net;

namespace Helpers.Elgato.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private delegate Models.RgbLightObject RgbLightUpdaterDelegate(Models.RgbLightObject light);

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public async Task<Models.LightObject> GetLightAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		var light = await _client.GetLightAsync(ip, cancellationToken);

		if (light.temperature is not null)
		{
			return (Models.WhiteLightObject)light;
		}

		return (Models.RgbLightObject)light;
	}

	public async Task SetBrightnessAsync(IPAddress ip, float brightness, CancellationToken? cancellationToken = null)
	{
		var light = await GetLightAsync(ip, cancellationToken);
		var updated = light with { Brightness = brightness, };
		await SetLightAsync(ip, updated, cancellationToken);
	}

	public async Task SetColorAsync(IPAddress ip, Color color, CancellationToken? cancellationToken = null)
	{
		var light = await GetLightAsync(ip, cancellationToken);
		if (light is not Models.RgbLightObject rgb) return;
		var updated = rgb with { Red = color.R, Green = color.G, Blue = color.B, };
		await SetLightAsync(ip, updated, cancellationToken);
	}

	public async Task SetKelvinsAsync(IPAddress ip, short kelvins, CancellationToken? cancellationToken = null)
	{
		var light = await GetLightAsync(ip, cancellationToken);
		if (light is not Models.WhiteLightObject white) return;
		var updated = white with { Kelvins = kelvins, };
		await SetLightAsync(ip, updated, cancellationToken);
	}

	public Task SetLightAsync(IPAddress ip, Models.LightObject light, CancellationToken? cancellationToken = null)
		=> _client.SetLightAsync(ip, (Models.Generated.LightObject)light, cancellationToken);

	public async Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = null)
	{
		var light = await GetLightAsync(ip, cancellationToken);
		var updated = light with { On = on, };
		await SetLightAsync(ip, updated, cancellationToken);
	}

	public async Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		var light = await GetLightAsync(ip, cancellationToken);
		var updated = light with { On = !light.On, };
		await SetLightAsync(ip, updated, cancellationToken);
	}
}
