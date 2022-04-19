namespace Helpers.Elgato;

public interface IElgatoService
{
	Task<Models.AccessoryInfoObject> GetLightInfoAsync(CancellationToken? cancellationToken = default);
	Task<(bool on, double brightness, short kelvins)> GetLightSettingsAsync(CancellationToken? cancellationToken = default);
	Task SetBrightnessAsync(double brightness, CancellationToken? cancellationToken = default);
	Task SetLightSettingsAsync(byte? on = default, byte? brightness = default, short? temperature = default, CancellationToken? cancellationToken = default);
	Task SetPowerStateAsync(bool on, CancellationToken? cancellationToken = default);
	Task SetTemperatureAsync(int kelvins, CancellationToken? cancellationToken = default);
	Task TogglePowerStateAsync(CancellationToken? cancellationToken = default);
}
