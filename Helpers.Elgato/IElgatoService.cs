using System.Net;

namespace Helpers.Elgato;

public interface IElgatoService
{
	Task<Models.AccessoryInfoObject> GetLightInfoAsync(IPAddress ip, CancellationToken? cancellationToken = default);
	Task<(bool on, double brightness, short kelvins)> GetLightSettingsAsync(IPAddress ip, CancellationToken? cancellationToken = default);
	Task SetBrightnessAsync(IPAddress ip, double brightness, CancellationToken? cancellationToken = default);
	Task SetLightSettingsAsync(IPAddress ip, byte? on = default, byte? brightness = default, short? temperature = default, CancellationToken? cancellationToken = default);
	Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = default);
	Task SetTemperatureAsync(IPAddress ip, int kelvins, CancellationToken? cancellationToken = default);
	Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = default);
}
