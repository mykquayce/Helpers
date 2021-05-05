using Dawn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace Helpers.OldhamCouncil.Concrete
{
	public class Client : IClient
	{
		private readonly HttpClient _httpClient;
		private readonly Encoding _encoding;

		private readonly static XmlSerializerFactory _xmlSerializerFactory = new();
		private readonly static JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, };
		private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline;
		private readonly static Regex regex = new(@"<table.+?<\/table>", _regexOptions);

		public Client(HttpClient httpClient, Encoding encoding)
		{
			_httpClient = Guard.Argument(() => httpClient).NotNull().Value;
			_encoding = Guard.Argument(() => encoding).NotNull().Value;
		}

		public async IAsyncEnumerable<KeyValuePair<long, string>> GetAddressesAsync(string postcode, CancellationToken? cancellationToken = default)
		{
			var uri = new Uri("/Forms/Common/GetSelectAddressList?type=Postcode&convert=true&term=" + postcode, UriKind.Relative);
			await using var stream = await _httpClient.GetStreamAsync(uri, cancellationToken ?? CancellationToken.None);
			var kvps = await JsonSerializer.DeserializeAsync<IEnumerable<KeyValuePair<string, string>>>(stream, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None);
			foreach (var kvp in kvps!)
			{
				if (cancellationToken?.IsCancellationRequested ?? false)
				{
					yield break;
				}

				var id = long.Parse(kvp.Key);
				var address = kvp.Value;

				yield return new(id, address);
			}
		}

		public async IAsyncEnumerable<Models.Generated.tableType> GetBinCollectionsAsync(long id, CancellationToken? cancellationToken = default)
		{
			var uri = new Uri("/Forms/EnvironmentalHealth/GetBinView?uprn=" + id, UriKind.Relative);
			var html = await _httpClient.GetStringAsync(uri, cancellationToken ?? CancellationToken.None);
			var matches = regex.Matches(html);

			foreach (Match match in matches)
			{
				if (cancellationToken?.IsCancellationRequested ?? false)
				{
					yield break;
				}

				var tableHtml = match.Value;
				var table = DeserializeXml<Models.Generated.tableType>(tableHtml);
				if (table is not null)
				{
					yield return table;
				}
			}
		}

		public T? DeserializeXml<T>(string xml)
			where T : class
		{
			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
			var bytes = _encoding.GetBytes(xml);
			using var stream = new MemoryStream(bytes);
			var t = serializer.Deserialize(stream) as T;
			return t;
		}
	}
}
