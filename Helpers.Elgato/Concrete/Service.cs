using Dawn;
using System.Drawing;
using System.Net;

namespace Helpers.Elgato.Concrete;

public class Service : IService
{
	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	private async IAsyncEnumerable<(bool on, float brightness, Color? color, short? kelvins)> GetLightsAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
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

	public IAsyncEnumerable<Models.Lights.LightModel> GetLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(ip, cancellationToken)
			.Select(tuple => new Models.Lights.LightModel(tuple.on, tuple.brightness));
	}

	public IAsyncEnumerable<Models.Lights.RgbLightModel> GetRgbLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(ip, cancellationToken)
			.Where(tuple => tuple.color.HasValue)
			.Select(tuple => new Models.Lights.RgbLightModel(tuple.on, tuple.brightness, tuple.color!.Value));
	}

	public IAsyncEnumerable<Models.Lights.WhiteLightModel> GetWhiteLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		return GetLightsAsync(ip, cancellationToken)
			.Where(tuple => tuple.kelvins.HasValue)
			.Select(tuple => new Models.Lights.WhiteLightModel(tuple.on, tuple.brightness, tuple.kelvins!.Value));
	}

	public Task SetBrightnessAsync(IPAddress ip, float brightness, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(brightness).InRange(0, 1);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = 1, brightness = (brightness * 100f).Round(), };
		return SetLightAsync(ip, func, cancellationToken);
	}

	public Task SetColorAsync(IPAddress ip, Color color, CancellationToken? cancellationToken = null)
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

		return SetLightAsync(ip, func, cancellationToken);
	}

	public Task SetKelvinsAsync(IPAddress ip, short kelvins, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(kelvins).InRange((short)2_900, (short)7_000);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = 1, temperature = kelvins.ConvertFromKelvinToElgato(), };
		return SetLightAsync(ip, func, cancellationToken);
	}

	public Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = null)
	{
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = on ? 1 : 0, };
		return SetLightAsync(ip, func, cancellationToken);
	}

	public async Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = null)
	{
		var lights = GetLightStatusAsync(ip, cancellationToken);
		var on = await lights.AnyAsync(l => l.On, cancellationToken ?? CancellationToken.None);
		Models.Generated.LightObject func(Models.Generated.LightObject light) => light with { on = on ? 0 : 1, };
		await SetLightAsync(ip, func, cancellationToken);
	}

	private async Task SetLightAsync(IPAddress ip, Func<Models.Generated.LightObject, Models.Generated.LightObject> func, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(ip).NotNull();
		Guard.Argument(func).NotNull();
		var lights = _client.GetLightsAsync(ip, cancellationToken);
		var updated = await lights.Select(func)
			.ToArrayAsync(cancellationToken ?? CancellationToken.None);
		await _client.SetLightAsync(ip, updated, cancellationToken);
	}
}
