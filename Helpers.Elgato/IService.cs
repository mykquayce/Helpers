using System.Drawing;
using System.Net;

namespace Helpers.Elgato;

public interface IService
{
	IAsyncEnumerable<Models.Lights.LightModel> GetLightStatusAsync(IPAddress ip, CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.Lights.RgbLightModel> GetRgbLightStatusAsync(IPAddress ip, CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.Lights.WhiteLightModel> GetWhiteLightStatusAsync(IPAddress ip, CancellationToken cancellationToken = default);
	Task SetBrightnessAsync(IPAddress ip, float brightness, CancellationToken cancellationToken = default);
	Task SetColorAsync(IPAddress ip, Color color, CancellationToken cancellationToken = default);
	Task SetKelvinsAsync(IPAddress ip, short kelvins, CancellationToken cancellationToken = default);
	Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken cancellationToken = default);
	Task TogglePowerStateAsync(IPAddress ip, CancellationToken cancellationToken = default);
}
