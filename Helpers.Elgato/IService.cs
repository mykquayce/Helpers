using System.Drawing;
using System.Net;

namespace Helpers.Elgato;

public interface IService
{
	IAsyncEnumerable<Models.Lights.LightModel> GetLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<Models.Lights.RgbLightModel> GetRgbLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<Models.Lights.WhiteLightModel> GetWhiteLightStatusAsync(IPAddress ip, CancellationToken? cancellationToken = null);
	Task SetBrightnessAsync(IPAddress ip, float brightness, CancellationToken? cancellationToken = null);
	Task SetColorAsync(IPAddress ip, Color color, CancellationToken? cancellationToken = null);
	Task SetKelvinsAsync(IPAddress ip, short kelvins, CancellationToken? cancellationToken = null);
	Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = null);
	Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = null);
}
