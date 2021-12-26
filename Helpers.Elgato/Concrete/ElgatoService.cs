using Dawn;
using System.Net;

namespace Helpers.Elgato.Concrete;

public class ElgatoService : IElgatoService
{
	private readonly IElgatoClient _client;

	public ElgatoService(IElgatoClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public Task<Models.AccessoryInfoObject> GetLightInfoAsync(IPAddress ip, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ip).NotNull();
		return _client.GetAccessoryInfoAsync(ip, cancellationToken);
	}

	public async Task<(bool on, double brightness, short kelvins)> GetLightSettingsAsync(IPAddress ip, CancellationToken? cancellationToken = default)
	{
		var light = await _client.GetLightAsync(ip, cancellationToken)
			.FirstAsync(cancellationToken ?? CancellationToken.None);

		var o = light.on == 1;
		var range = new Range(0, 100);
		var b = range.ReduceValueToFraction(light.brightness);
		var k = (short)((int)light.temperature).ConvertFromElgatoToKelvin();

		return (o, b, k);
	}

	public Task SetBrightnessAsync(IPAddress ip, double brightness, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ip).NotNull();
		Guard.Argument(brightness).InRange(0, 1);
		var range = new Range(0, 100);
		var b = (byte)range.IncreaseValueFromFraction(brightness);
		return SetLightSettingsAsync(ip, brightness: b, cancellationToken: cancellationToken);
	}

	public async Task SetLightSettingsAsync(IPAddress ip, byte? on = default, byte? brightness = default, short? temperature = default, CancellationToken? cancellationToken = default)
	{
		if (on is null
			|| brightness is null
			|| temperature is null)
		{
			var current = await _client.GetLightAsync(ip, cancellationToken)
				.FirstAsync(cancellationToken ?? CancellationToken.None);

			on ??= current.on;
			brightness ??= current.brightness;
			temperature ??= current.temperature;
		}

		var light = new Models.MessageObject.LightObject(on!.Value, brightness!.Value, temperature!.Value);

		await _client.SetLightAsync(ip, light, cancellationToken);
	}

	public Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ip).NotNull();
		return SetLightSettingsAsync(ip, on: on ? (byte)1 : (byte)0, cancellationToken: cancellationToken);
	}

	public Task SetTemperatureAsync(IPAddress ip, int kelvins, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ip).NotNull();
		Guard.Argument(kelvins).InRange(2_900, 7_000);
		var temperature = (short)kelvins.ConvertFromKelvinToElgato();
		return SetLightSettingsAsync(ip, temperature: temperature, cancellationToken: cancellationToken);
	}

	public async Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(ip).NotNull();
		var light = await _client.GetLightAsync(ip, cancellationToken)
			.FirstAsync(cancellationToken ?? CancellationToken.None);
		light = light with { on = light.on == 1 ? (byte)0 : (byte)1, };
		await _client.SetLightAsync(ip, light, cancellationToken);
	}
}
