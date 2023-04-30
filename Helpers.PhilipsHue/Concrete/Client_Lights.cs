using Dawn;
using System.Drawing;

namespace Helpers.PhilipsHue.Concrete;

public partial class Client
{
	public async IAsyncEnumerable<KeyValuePair<string, int>> GetLightAliasesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestUri = _uriPrefix + "/lights";
		var dictionary = await GetFromJsonAsync<Dictionary<int, Light>>(requestUri, cancellationToken);

		foreach (var (index, light) in dictionary)
		{
			var alias = light.name;
			yield return new(alias, index);
		}
	}

	#region brightness
	public async Task<float> GetLightBrightnessAsync(int index, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"{_uriPrefix}/lights/{index:D}";
		((_, var brightness, _, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return (float)brightness / byte.MaxValue;
	}

	public Task SetLightBrightnessAsync(int index, float brightness, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(brightness).InRange(0, 1);
		var body = new { on = true, bri = (byte)(brightness * byte.MaxValue), };
		var requestUri = $"{_uriPrefix}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion brightness

	#region power
	public async Task<bool> GetLightPowerAsync(int index, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"{_uriPrefix}/lights/{index:D}";
		((var power, _, _, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return power;
	}

	public Task SetLightPowerAsync(int index, bool on, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		var body = new { on, };
		var requestUri = $"{_uriPrefix}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion power

	#region temperature
	public async Task<short> GetLightTemperatureAsync(int index, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"{_uriPrefix}/lights/{index:D}";
		((_, _, var ct, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return (short)(1_000_000d / ct);
	}

	public Task SetLightTemperatureAsync(int index, short kelvins, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(kelvins).InRange((short)2_900, (short)7_000);
		var mired = 1_000_000 / kelvins;
		var body = new { on = true, ct = mired, };
		var requestUri = $"{_uriPrefix}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion temperature

	#region color
	public async Task<Color> GetLightColorAsync(int index, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"{_uriPrefix}/lights/{index:D}";
		((_, var bri, _, (var x, var y)), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return new PointF(x: x, y: y).ToColor(bri);
	}

	public Task SetLightColorAsync(int index, Color color, CancellationToken cancellationToken = default)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(color).NotDefault();
		var ((x, y), bri) = color.ToXY();
		var body = new { on = true, bri, xy = new[] { x, y, }, };
		var requestUri = $"{_uriPrefix}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion color
}
