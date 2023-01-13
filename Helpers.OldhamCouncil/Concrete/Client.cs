using Dawn;
using Helpers.Web;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Helpers.OldhamCouncil.Concrete
{
	public partial class Client : WebClientBase, IClient
	{
		private readonly Encoding _encoding = Encoding.UTF8;

		private readonly static XmlSerializerFactory _xmlSerializerFactory = new();

		public Client(HttpClient httpClient) : base(httpClient) { }

		public async IAsyncEnumerable<Models.Address> GetAddressesAsync(string postcode, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			Guard.Argument(postcode).NotNull().NotEmpty().NotWhiteSpace();
			var uri = new Uri("Common/GetAddressList?type=Postcode&term=" + HttpUtility.UrlEncode(postcode), UriKind.Relative);
			var (headers, status, addresses) = await SendAsync<ICollection<Models.Address>>(HttpMethod.Get, uri);

			using var enumerator = addresses!.GetEnumerator();

			enumerator.MoveNext();

			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public async IAsyncEnumerable<Models.Generated.tableType> GetBinCollectionsAsync(string uprn, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			Guard.Argument(uprn).NotNull().NotEmpty();
			var uri = new Uri("bincollectiondates/details?uprn=" + uprn, UriKind.Relative);
			var (_, _, html) = await SendAsync<string>(HttpMethod.Get, uri);
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
}
