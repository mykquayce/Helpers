using System.Drawing;
using System.Net;

namespace Helpers.Elgato;

public interface IService
{
	Task<Models.LightObject> GetLightAsync(IPAddress ip, CancellationToken? cancellationToken = null);
	Task SetBrightnessAsync(IPAddress ip, float brightness, CancellationToken? cancellationToken = null);
	Task SetColorAsync(IPAddress ip, Color color, CancellationToken? cancellationToken = null);
	Task SetKelvinsAsync(IPAddress ip, short kelvins, CancellationToken? cancellationToken = null);
	Task SetLightAsync(IPAddress ip, Models.LightObject light, CancellationToken? cancellationToken = null);
	Task SetPowerStateAsync(IPAddress ip, bool on, CancellationToken? cancellationToken = null);
	Task TogglePowerStateAsync(IPAddress ip, CancellationToken? cancellationToken = null);
}
