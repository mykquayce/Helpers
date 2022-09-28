using System.Drawing;

namespace Helpers.PhilipsHue;

public interface IClient
{
	IAsyncEnumerable<KeyValuePair<string, int>> GetLightAliasesAsync(Uri? baseAddress, CancellationToken? cancellationToken = null);

	Task<float> GetLightBrightnessAsync(int index, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task<Color> GetLightColorAsync(int index, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task<bool> GetLightPowerAsync(int index, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task<short> GetLightTemperatureAsync(int index, Uri? baseAddress, CancellationToken? cancellationToken = null);

	Task SetLightBrightnessAsync(int index, float brightness, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task SetLightColorAsync(int index, Color color, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task SetLightPowerAsync(int index, bool on, Uri? baseAddress, CancellationToken? cancellationToken = null);
	Task SetLightTemperatureAsync(int index, short kelvins, Uri? baseAddress, CancellationToken? cancellationToken = null);
}
