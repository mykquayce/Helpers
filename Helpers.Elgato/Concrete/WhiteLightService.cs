using Helpers.Elgato.Models;

namespace Helpers.Elgato.Concrete;

public class WhiteLightService(IWhiteLightClient client) : IWhiteLightService
{
	public Task<WhiteLight> GetAsync(CancellationToken cancellationToken = default)
		=> client.GetAsync(cancellationToken);

	public Task<Info> GetInfoAsync(CancellationToken cancellationToken = default)
		=> client.GetInfoAsync(cancellationToken);

	public async Task SetBrightnessAsync(byte brightness, CancellationToken cancellationToken = default)
	{
		var light = await client.GetAsync(cancellationToken);
		var @new = light with { brightness = brightness, };
		await client.SetAsync(@new, cancellationToken);
	}

	public async Task SetPowerStateAsync(bool on, CancellationToken cancellationToken = default)
	{
		var light = await client.GetAsync(cancellationToken);
		var @new = light with { on = on ? (byte)1 : (byte)0, };
		await client.SetAsync(@new, cancellationToken);
	}

	public async Task SetTemperatureAsync(short temperature, CancellationToken cancellationToken = default)
	{
		var light = await client.GetAsync(cancellationToken);
		var @new = light with { temperature = temperature, };
		await client.SetAsync(@new, cancellationToken);
	}

	public async Task TogglePowerStateAsync(CancellationToken cancellationToken = default)
	{
		var light = await client.GetAsync(cancellationToken);
		var @new = light with { on = light.on > 0 ? (byte)0 : (byte)1, };
		await client.SetAsync(@new, cancellationToken);
	}
}
