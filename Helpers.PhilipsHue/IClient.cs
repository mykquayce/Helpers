using System.Drawing;

namespace Helpers.PhilipsHue;

public interface IClient
{
	IAsyncEnumerable<KeyValuePair<string, int>> GetLightAliasesAsync(CancellationToken cancellationToken = default);

	Task<float> GetLightBrightnessAsync(int index, CancellationToken cancellationToken = default);
	Task<Color> GetLightColorAsync(int index, CancellationToken cancellationToken = default);
	Task<bool> GetLightPowerAsync(int index, CancellationToken cancellationToken = default);
	Task<short> GetLightTemperatureAsync(int index, CancellationToken cancellationToken = default);

	Task SetLightBrightnessAsync(int index, float brightness, CancellationToken cancellationToken = default);
	Task SetLightColorAsync(int index, Color color, CancellationToken cancellationToken = default);
	Task SetLightPowerAsync(int index, bool on, CancellationToken cancellationToken = default);
	Task SetLightTemperatureAsync(int index, short kelvins, CancellationToken cancellationToken = default);

	Task ApplySceneToGroupAsync(int group, string scene, TimeSpan transition, CancellationToken cancellationToken = default);
	IAsyncEnumerable<KeyValuePair<string, int>> GetGroupsAsync(CancellationToken cancellationToken = default);

	IAsyncEnumerable<KeyValuePair<string, string>> GetScenesAsync(CancellationToken cancellationToken = default);
}
