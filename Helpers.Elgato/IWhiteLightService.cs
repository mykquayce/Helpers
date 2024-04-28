using Helpers.Elgato.Models;

namespace Helpers.Elgato;
public interface IWhiteLightService
{
	Task<WhiteLight> GetAsync(CancellationToken cancellationToken = default);
	Task<Info> GetInfoAsync(CancellationToken cancellationToken = default);
	Task SetBrightnessAsync(byte brightness, CancellationToken cancellationToken = default);
	Task SetPowerStateAsync(bool on, CancellationToken cancellationToken = default);
	Task SetTemperatureAsync(short temperature, CancellationToken cancellationToken = default);
	Task TogglePowerStateAsync(CancellationToken cancellationToken = default);
}
