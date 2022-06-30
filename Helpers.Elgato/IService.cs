using System.Drawing;

namespace Helpers.Elgato;

public interface IService
{
	IAsyncEnumerable<Models.Lights.LightModel> GetLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<Models.Lights.RgbLightModel> GetRgbLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<Models.Lights.WhiteLightModel> GetWhiteLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	Task SetBrightnessAsync(string alias, float brightness, CancellationToken? cancellationToken = null);
	Task SetColorAsync(string alias, Color color, CancellationToken? cancellationToken = null);
	Task SetKelvinsAsync(string alias, short kelvins, CancellationToken? cancellationToken = null);
	Task SetPowerStateAsync(string alias, bool on, CancellationToken? cancellationToken = null);
	Task TogglePowerStateAsync(string alias, CancellationToken? cancellationToken = null);
}
