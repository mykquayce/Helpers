using System.Drawing;

namespace Helpers.PhilipsHue;

public interface IService
{
	IAsyncEnumerable<string> GetLightAliasesAsync(CancellationToken cancellationToken = default);

	Task<float> GetLightBrightnessAsync(string alias, CancellationToken cancellationToken = default);
	Task<Color> GetLightColorAsync(string alias, CancellationToken cancellationToken = default);
	Task<bool> GetLightPowerAsync(string alias, CancellationToken cancellationToken = default);
	Task<short> GetLightTemperatureAsync(string alias, CancellationToken cancellationToken = default);

	Task SetLightBrightnessAsync(string alias, float brightness, CancellationToken cancellationToken = default);
	Task SetLightColorAsync(string alias, Color color, CancellationToken cancellationToken = default);
	Task SetLightPowerAsync(string alias, bool on, CancellationToken cancellationToken = default);
	Task SetLightTemperatureAsync(string alias, short kelvins, CancellationToken cancellationToken = default);

	Task ApplySceneToGroupAsync(string scene, string group, TimeSpan transition = default, CancellationToken cancellationToken = default);
}
