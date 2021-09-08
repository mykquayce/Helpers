using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Helpers.Cineworld.Concrete;

public class Client : IClient
{
	#region config
	public record Config(
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri BaseAddressUri,
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri AllPerformancesUri,
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri ListingsUri,
		[property: JsonConverter(typeof(Helpers.Json.Converters.JsonTimeSpanConverter))] TimeSpan CacheExpiration)
		: IOptions<Config>
	{
		public readonly static Uri DefaultBaseAddressUri = new("https://www.cineworld.co.uk", UriKind.Absolute);
		public readonly static Uri DefaultAllPerformancesUrii = new("/syndication/all-performances.xml", UriKind.Relative);
		public readonly static Uri DefaultListingsUri = new("/syndication/listings.xml", UriKind.Relative);
		public readonly static TimeSpan DefaultCacheExpiration = TimeSpan.FromHours(1);

		public Config() : this(DefaultBaseAddressUri, DefaultAllPerformancesUrii, DefaultListingsUri, DefaultCacheExpiration) { }

		public static Config Defaults => new();

		#region IOptions implementation
		public Config Value => this;
		#endregion IOptions implementation
	}
	#endregion config

	private readonly Uri _allPerformancesUri, _listingsUri;
	private readonly TimeSpan _cacheExpiration;
	private readonly HttpClient _httpClient;
	private readonly XmlSerializerFactory _xmlSerializerFactory;
	private readonly IMemoryCache _cache;

	public Client(IOptions<Config> options, HttpClient httpClient, XmlSerializerFactory xmlSerializerFactory, IMemoryCache cache)
	{
		var config = Guard.Argument(() => options).NotNull().Wrap(o => o.Value).NotNull().Value;
		_allPerformancesUri = Guard.Argument(() => config.AllPerformancesUri).NotNull().Require(uri => uri.OriginalString is not null).Value;
		_listingsUri = Guard.Argument(() => config.ListingsUri).NotNull().Require(uri => uri.OriginalString is not null).Value;
		_cacheExpiration = Guard.Argument(() => config.CacheExpiration).Positive().Value;
		_httpClient = Guard.Argument(() => httpClient).NotNull().Require(c => c.BaseAddress?.OriginalString is not null).Value;
		_xmlSerializerFactory = Guard.Argument(() => xmlSerializerFactory).NotNull().Value;
		_cache = Guard.Argument(() => cache).NotNull().Value;
	}

	public Task<Models.Generated.AllPerformances.cinemas> GetAllPerformancesAsync(CancellationToken? cancellationToken = default)
		=> GetAsync<Models.Generated.AllPerformances.cinemas>(_allPerformancesUri, cancellationToken);

	public Task<Models.Generated.Listings.cinemas> GetListingsAsync(CancellationToken? cancellationToken = default)
		=> GetAsync<Models.Generated.Listings.cinemas>(_listingsUri, cancellationToken);

	private async Task<T> GetAsync<T>(Uri relativeUri, CancellationToken? cancellationToken = default)
		where T : class
	{
		Guard.Argument(() => relativeUri).NotNull().Require(u => !u.IsAbsoluteUri);

		// try the cache
		if (_cache.TryGetValue<T>(relativeUri, out var result))
		{
			return result;
		}

		// try remote
		result = await GetFromRemoteAsync<T>(relativeUri, cancellationToken ?? CancellationToken.None);

		// cache
		var expirationTokenSource = new CancellationTokenSource(millisecondsDelay: (int)_cacheExpiration.TotalMilliseconds);
		var expirationToken = new CancellationChangeToken(expirationTokenSource.Token);
		_cache.Set(relativeUri, result, expirationToken);

		// return
		return result;
	}

	private async Task<T> GetFromRemoteAsync<T>(Uri relativeUri, CancellationToken? cancellationToken = default)
		where T : class
	{
		Guard.Argument(() => relativeUri)
			.NotNull()
			.Require(uri => uri.OriginalString is not null)
			.Require(u => !u.IsAbsoluteUri);

		using var response = await _httpClient.GetAsync(relativeUri, cancellationToken ?? CancellationToken.None);

		response.EnsureSuccessStatusCode();

		await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken ?? CancellationToken.None);

		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

		try
		{
			var t = serializer.Deserialize(stream) as T;
			return t!;
		}
		catch (Exception ex)
		{
			ex.Data.Add(nameof(Uri), _httpClient.BaseAddress!.OriginalString + relativeUri);
			ex.Data.Add(nameof(Type), typeof(T).Name);

			if (stream.CanSeek)
			{
				stream.Position = 0;
				using var reader = new StreamReader(stream);
				var json = await reader.ReadToEndAsync();
				ex.Data.Add(nameof(json), json);
			}

			throw;
		}
	}
}
