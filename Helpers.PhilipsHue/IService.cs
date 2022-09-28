using System.Drawing;

namespace Helpers.PhilipsHue;

public interface IService
{
	public Uri? BaseAddress { init; }

	IAsyncEnumerable<string> GetLightAliasesAsync(CancellationToken? cancellationToken = null);

	Task<float> GetLightBrightnessAsync(string alias, CancellationToken? cancellationToken = null);
	Task<Color> GetLightColorAsync(string alias, CancellationToken? cancellationToken = null);
	Task<bool> GetLightPowerAsync(string alias, CancellationToken? cancellationToken = null);
	Task<short> GetLightTemperatureAsync(string alias, CancellationToken? cancellationToken = null);

	Task SetLightBrightnessAsync(string alias, float brightness, CancellationToken? cancellationToken = null);
	Task SetLightColorAsync(string alias, Color color, CancellationToken? cancellationToken = null);
	Task SetLightPowerAsync(string alias, bool on, CancellationToken? cancellationToken = null);
	Task SetLightTemperatureAsync(string alias, short kelvins, CancellationToken? cancellationToken = null);
}
