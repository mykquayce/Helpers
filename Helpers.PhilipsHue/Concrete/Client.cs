using Dawn;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Net.Http.Json;
using System.Text.Json;

namespace Helpers.PhilipsHue.Concrete;

public class Client : IClient
{
	private readonly Config _config;
	private readonly HttpClient _httpClient;
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	public Client(IOptions<Config> options, HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
	{
		_config = options.Value;
		_httpClient = httpClient;
		_jsonSerializerOptions = jsonSerializerOptions;
	}

	public async IAsyncEnumerable<KeyValuePair<string, int>> GetLightAliasesAsync(CancellationToken? cancellationToken = null)
	{
		var requestUri = $"api/{_config.Username}/lights";
		var dictionary = await GetFromJsonAsync<Dictionary<int, Models.LightResponseObject>>(requestUri, cancellationToken);

		foreach (var (index, light) in dictionary)
		{
			var alias = light.name;
			yield return new(alias, index);
		}
	}

	#region brightness
	public async Task<float> GetLightBrightnessAsync(int index, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"api/{_config.Username}/lights/{index:D}";
		((_, var brightness, _, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return (float)brightness / byte.MaxValue;
	}

	public Task SetLightBrightnessAsync(int index, float brightness, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(brightness).InRange(0, 1);
		var body = new { on = true, bri = (byte)(brightness * byte.MaxValue), };
		var requestUri = $"api/{_config.Username}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion brightness

	#region power
	public async Task<bool> GetLightPowerAsync(int index, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"api/{_config.Username}/lights/{index:D}";
		((var power, _, _, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return power;
	}

	public Task SetLightPowerAsync(int index, bool on, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		var body = new { on, };
		var requestUri = $"api/{_config.Username}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion power

	#region temperature
	public async Task<short> GetLightTemperatureAsync(int index, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"api/{_config.Username}/lights/{index:D}";
		((_, _, var ct, _), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return (short)(1_000_000d / ct);
	}

	public Task SetLightTemperatureAsync(int index, short kelvins, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(kelvins).InRange((short)2_900, (short)7_000);
		var mired = 1_000_000 / kelvins;
		var body = new { on = true, ct = mired, };
		var requestUri = $"api/{_config.Username}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion temperature

	#region color
	public async Task<Color> GetLightColorAsync(int index, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		var requestUri = $"api/{_config.Username}/lights/{index:D}";
		((_, var bri, _, (var x, var y)), _) = await GetFromJsonAsync<Models.LightResponseObject>(requestUri, cancellationToken);
		return new PointF(x: x, y: y).ToColor(bri);
	}

	public Task SetLightColorAsync(int index, Color color, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(index).Positive();
		Guard.Argument(color).NotDefault();
		var ((x, y), bri) = color.ToXY();
		var body = new { on = true, bri, xy = new[] { x, y, }, };
		var requestUri = $"api/{_config.Username}/lights/{index:D}/state";
		return PutAsJsonAsync(requestUri, body, cancellationToken);
	}
	#endregion color

	private async Task<T> GetFromJsonAsync<T>(string requestUri, CancellationToken? cancellationToken = null)
	{
		return (await _httpClient.GetFromJsonAsync<T>(requestUri, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None))
			?? throw new Exceptions.DeserializationException<T>(requestUri);
	}

	private Task PutAsJsonAsync(string requestUri, object body, CancellationToken? cancellationToken = null)
	{
		return _httpClient.PutAsJsonAsync(requestUri, body, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None);
	}
}
