using Dawn;
using System.Drawing;

namespace Helpers.Elgato.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly NetworkDiscoveryApi.IService _networkDiscoveryApiService;

	public Service(
		IClient client,
		Helpers.NetworkDiscoveryApi.IService networkDiscoveryApiClient)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_networkDiscoveryApiService = networkDiscoveryApiClient;
	}

	private async IAsyncEnumerable<(bool on, float brightness, Color? color, short? kelvins)> GetLightsAsync(string alias, CancellationToken? cancellationToken = null)
	{
		(_, _, var ip, _, _) = await _networkDiscoveryApiService.GetLeaseAsync(alias, cancellationToken);
		var lights = _client.GetLightsAsync(ip, cancellationToken);

		await foreach (var (on, brightness, temperature, hue, saturation) in lights)
		{
			Color? color = hue.HasValue && saturation.HasValue
				? new HsbColor((float)hue!.Value, (float)(saturation!.Value / 100f), brightness / 100f).GetColor()
				: null;

			short? kelvins = temperature.HasValue
				? temperature!.Value.ConvertFromElgatoToKelvin()
				: null;

			yield return (on == 1, brightness / 100f, color, kelvins);
		}
	}

	public IAsyncEnumerable<Models.Lights.LightModel> GetLightStatusAsync(string alias, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(alias, cancellationToken)
			.Select(tuple => new Models.Lights.LightModel(tuple.on, tuple.brightness));
	}

	public IAsyncEnumerable<Models.Lights.RgbLightModel> GetRgbLightStatusAsync(string alias, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(alias, cancellationToken)
			.Where(tuple => tuple.color.HasValue)
			.Select(tuple => new Models.Lights.RgbLightModel(tuple.on, tuple.brightness, tuple.color!.Value));
	}

	public IAsyncEnumerable<Models.Lights.WhiteLightModel> GetWhiteLightStatusAsync(string alias, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(alias, cancellationToken)
			.Where(tuple => tuple.kelvins.HasValue)
			.Select(tuple => new Models.Lights.WhiteLightModel(tuple.on, tuple.brightness, tuple.kelvins!.Value));
	}

	public Task SetBrightnessAsync(string alias, float brightness, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(brightness).InRange(0, 1);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = 1, brightness = (brightness * 100f).Round(), };
		return SetLightAsync(alias, func, cancellationToken);
	}

	public Task SetColorAsync(string alias, Color color, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(color).NotDefault();
		Models.Generated.LightObject func(Models.Generated.LightObject light)
		{
			var hsbColor = color.GetHsbColor();

			return light with
			{
				on = 1,
				brightness = (hsbColor.Brightness * 100f).Round(),
				hue = hsbColor.Hue,
				saturation = hsbColor.Saturation * 100f,
			};
		}

		return SetLightAsync(alias, func, cancellationToken);
	}

	public Task SetKelvinsAsync(string alias, short kelvins, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(kelvins).InRange((short)2_900, (short)7_000);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = 1, temperature = kelvins.ConvertFromKelvinToElgato(), };
		return SetLightAsync(alias, func, cancellationToken);
	}

	public Task SetPowerStateAsync(string alias, bool on, CancellationToken? cancellationToken = null)
	{
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = on ? 1 : 0, };
		return SetLightAsync(alias, func, cancellationToken);
	}

	public async Task TogglePowerStateAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var lights = GetLightStatusAsync(alias, cancellationToken);
		var on = await lights.AnyAsync(l => l.On, cancellationToken ?? CancellationToken.None);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = on ? 0 : 1, };
		await SetLightAsync(alias, func, cancellationToken);
	}

	private async Task SetLightAsync(string alias, Func<Models.Generated.LightObject, Models.Generated.LightObject> func, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(alias).NotNull().NotEmpty().NotWhiteSpace();
		Guard.Argument(func).NotNull();
		(_, _, var ip, _, _) = await _networkDiscoveryApiService.GetLeaseAsync(alias, cancellationToken);
		var lights = _client.GetLightsAsync(ip, cancellationToken);
		var updated = await lights.Select(func)
			.ToArrayAsync(cancellationToken ?? CancellationToken.None);
		await _client.SetLightAsync(ip, updated, cancellationToken);
	}
}
