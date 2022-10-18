using System.Drawing;

namespace Helpers.PhilipsHue;

public interface IClient
{
	IAsyncEnumerable<KeyValuePair<string, int>> GetLightAliasesAsync(CancellationToken? cancellationToken = null);

	Task<float> GetLightBrightnessAsync(int index, CancellationToken? cancellationToken = null);
	Task<Color> GetLightColorAsync(int index, CancellationToken? cancellationToken = null);
	Task<bool> GetLightPowerAsync(int index, CancellationToken? cancellationToken = null);
	Task<short> GetLightTemperatureAsync(int index, CancellationToken? cancellationToken = null);

	Task SetLightBrightnessAsync(int index, float brightness, CancellationToken? cancellationToken = null);
	Task SetLightColorAsync(int index, Color color, CancellationToken? cancellationToken = null);
	Task SetLightPowerAsync(int index, bool on, CancellationToken? cancellationToken = null);
	Task SetLightTemperatureAsync(int index, short kelvins, CancellationToken? cancellationToken = null);
}
