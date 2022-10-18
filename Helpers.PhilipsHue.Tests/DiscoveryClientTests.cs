using Helpers.PhilipsHue.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.PhilipsHue.Tests;

public class DiscoveryClientTests
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "errors with too many requests")]
	[Theory(Skip = "errors with too many requests")]
	[InlineData("https://discovery.meethue.com/", 1_000)]
	public async Task GetBridgeIPAddressTests(string baseAddress, int timeout)
	{
		IPAddress ip;
		{
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());
			using var httpHandler = new HttpClientHandler { AllowAutoRedirect = false, };
			using var httpClient = new HttpClient(httpHandler) { BaseAddress = new Uri(baseAddress), };
			IDiscoveryClient sut = new Concrete.DiscoveryClient(memoryCache, httpClient);
			using var cts = new CancellationTokenSource(millisecondsDelay: timeout);
			ip = await sut.GetBridgeIPAddressAsync(cts.Token);
		}
		Assert.NotNull(ip);
		Assert.NotEqual(IPAddress.None, ip);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "errors with too many requests")]
	[Theory(Skip = "errors with too many requests")]
	[InlineData("https://discovery.meethue.com/", 10)]
	public async Task CacheTests(string baseAddress, int count)
	{
		var times = new List<double>();
		{
			var stopwatch = new Stopwatch();
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());
			using var httpHandler = new HttpClientHandler { AllowAutoRedirect = false, };
			using var httpClient = new HttpClient(httpHandler) { BaseAddress = new Uri(baseAddress), };
			IDiscoveryClient sut = new Concrete.DiscoveryClient(memoryCache, httpClient);
			while (count-- > 0)
			{
				stopwatch.Restart();
				try { await sut.GetBridgeIPAddressAsync(); }
				catch { }
				stopwatch.Stop();
				times.Add(stopwatch.Elapsed.TotalMilliseconds);
			}
		}
		Assert.NotEmpty(times);
		Assert.InRange(times.First(), 100d, 1_000d);
		Assert.All(times.Skip(1), l => Assert.InRange(l, 0d, 1d));
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "errors with too many requests")]
	[Theory(Skip = "errors with too many requests")]
	[InlineData("https://discovery.meethue.com/", 1_000)]
	public async Task DependencyInjectionTests(string discoveryEndPoint, int timeout)
	{
		IPAddress ip;
		{
			IServiceProvider serviceProvider;
			{
				IConfiguration configuration;
				{
					var initialData = new Dictionary<string, string?>
					{
						[nameof(discoveryEndPoint)] = discoveryEndPoint,
					};

					configuration = new ConfigurationBuilder()
						.AddInMemoryCollection(initialData)
						.Build();
				}

				serviceProvider = new ServiceCollection()
					.Configure<Config>(configuration)
					.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
					.AddHttpClient<IDiscoveryClient, Concrete.DiscoveryClient>(name: "philipshue-discovery-client", (provider, client) =>
					{
						var config = provider.GetRequiredService<IOptions<Config>>().Value;
						client.BaseAddress = config.DiscoveryEndPoint;
					})
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
					.Services
					.BuildServiceProvider();
			}

			var sut = serviceProvider.GetRequiredService<IDiscoveryClient>();

			using var cts = new CancellationTokenSource(timeout);
			ip = await sut.GetBridgeIPAddressAsync(cts.Token);

			await ((IAsyncDisposable)serviceProvider).DisposeAsync();
		}

		// Assert
		Assert.NotNull(ip);
		Assert.NotEqual(IPAddress.None, ip);
	}

	[Theory]
	[InlineData("discoveryendpoint", "http://localhost/")]
	[InlineData("discoveryEndPoint", "http://localhost/")]
	[InlineData("DiscoveryEndPoint", "http://localhost/")]
	[InlineData("DISCOVERYENDPOINT", "http://localhost/")]
	public void DependencyInjectDiscoveryEndPointTests(string key, string value)
	{
		IOptions<Config> options;
		{
			var initialData = new Dictionary<string, string?>
			{
				[key] = value,
			};

			IConfiguration configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();

			using var serviceProvider = new ServiceCollection()
				.Configure<Config>(configuration)
				.BuildServiceProvider();

			options = serviceProvider.GetRequiredService<IOptions<Config>>();
		}

		Assert.NotNull(options);
		Assert.NotNull(options.Value);
		Assert.NotNull(options.Value.DiscoveryEndPoint);
		Assert.NotEqual(Config.DefaultDiscoveryEndPoint, options.Value.DiscoveryEndPoint);
		Assert.Equal(value, options.Value.DiscoveryEndPoint.ToString());
	}

	[Theory]
	[InlineData(
		@"[{""id"":""ecb5fafffe18e324"",""internalipaddress"":""192.168.1.156"",""port"":443}]",
		17_056_815_141_122_269_988, new byte[4] { 192, 168, 1, 156, }, 443)]
	public async Task DeserializationTests(
		string json,
		ulong expectedId, byte[] expectedInternalIPAddress, ushort expectedPort)
	{
		IReadOnlyList<DiscoveryResponseObject>? responses;
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			await using var stream = new MemoryStream(bytes);
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
			responses = await JsonSerializer.DeserializeAsync<IReadOnlyList<DiscoveryResponseObject>>(stream, options);
		}

		Assert.NotNull(responses);
		Assert.Single(responses);
		Assert.DoesNotContain(null, responses);
		Assert.Equal(expectedId, responses[0].Id);
		Assert.Equal(new IPAddress(expectedInternalIPAddress), responses[0].InternalIPAddress);
		Assert.Equal(expectedPort, responses[0].Port);
	}

	[Theory]
	[InlineData("ecb5fafffe18e324", 17_056_815_141_122_269_988)]
	public void HexConversionTests(string before, ulong expected)
	{
		var actual = ulong.Parse(before, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		Assert.Equal(expected, actual);

		var bytes = Convert.FromHexString(before);
		var integer = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
		Assert.Equal(expected, integer);
	}

	[Theory]
	[InlineData(
		@"{""ulong"":""ecb5fafffe18e324"",""short"":""7fff""}",
		17_056_815_141_122_269_988, 32_767)]
	[InlineData(
		@"{""ulong"":""ecb5fafffe18e324"",""short"":""8000""}",
		17_056_815_141_122_269_988, -32_768)]
	public async void JsonHexConversionTests(string json, ulong expectedUlong, short expectedShort)
	{
		Record? record;
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			await using var stream = new MemoryStream(bytes);
			record = await JsonSerializer.DeserializeAsync<Record>(stream);
		}

		Assert.NotNull(record);
		Assert.Equal(expectedUlong, record.@ulong);
		Assert.Equal(expectedShort, record.@short);

		string? backJson;
		{
			await using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, record);
			stream.Position = 0;
			using var reader = new StreamReader(stream);
			backJson = await reader.ReadToEndAsync();
		}

		Assert.Equal(json, backJson);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	private record Record(
		[property: JsonConverter(typeof(Converters.JsonHexConverter<ulong>))] ulong @ulong,
		[property: JsonConverter(typeof(Converters.JsonHexConverter<short>))] short @short);
}
