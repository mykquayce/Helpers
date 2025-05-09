﻿using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Helpers.OldhamCouncil.Concrete;

public partial class Client(HttpClient httpClient) : IClient
{
	private readonly Encoding _encoding = Encoding.UTF8;

	private readonly static XmlSerializerFactory _xmlSerializerFactory = new();

	public IAsyncEnumerable<Models.Address> GetAddressesAsync(string postcode, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(postcode);
		var uri = new Uri("Common/GetAddressList?type=Postcode&term=" + HttpUtility.UrlEncode(postcode), UriKind.Relative);
		return httpClient.GetFromJsonAsAsyncEnumerable<Models.Address>(uri, cancellationToken);
	}

	public async IAsyncEnumerable<Models.Generated.tableType> GetBinCollectionsAsync(string uprn, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(uprn);
		var uri = new Uri("bincollectiondates/details?uprn=" + uprn, UriKind.Relative);
		var html = await httpClient.GetStringAsync(uri);
		var matches = TableRegex().Matches(html!);

		foreach (Match match in matches)
		{
			if (cancellationToken.IsCancellationRequested)
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

	[GeneratedRegex("<table.+?<\\/table>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
	private static partial Regex TableRegex();
}
