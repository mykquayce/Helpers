using System.Drawing;
using System.Net;

namespace Helpers.Elgato;

public interface IService
{
	IAsyncEnumerable<(bool on, float brightness, Color? color, short? kelvins)> GetLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<(bool on, float brightness, Color color)> GetRgbLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	IAsyncEnumerable<(bool on, float brightness, short kelvins)> GetWhiteLightStatusAsync(string alias, CancellationToken? cancellationToken = null);
	Task SetBrightnessAsync(string alias, float brightness, CancellationToken? cancellationToken = null);
	Task SetColorAsync(string alias, Color color, CancellationToken? cancellationToken = null);
	Task SetKelvinsAsync(string alias, short kelvins, CancellationToken? cancellationToken = null);
	Task SetPowerStateAsync(string alias, bool on, CancellationToken? cancellationToken = null);
	Task TogglePowerStateAsync(string alias, CancellationToken? cancellationToken = null);
}
