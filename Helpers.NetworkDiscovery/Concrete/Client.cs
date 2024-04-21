using Helpers.Networking.Models;
using System.Net.Http.Json;
using System.Web;

namespace Helpers.NetworkDiscovery.Concrete;

public class Client(HttpClient httpClient) : IClient
{
	public IAsyncEnumerable<DhcpLease> GetAllLeasesAsync(CancellationToken cancellationToken = default)
		=> httpClient.GetFromJsonAsAsyncEnumerable<DhcpLease>("api/router", cancellationToken);

	public Task<HttpResponseMessage> ResetAsync(CancellationToken cancellationToken = default)
		=> httpClient.PutAsync("api/router/reset", content: null, cancellationToken: cancellationToken);

	public Task<DhcpLease> ResolveAsync(object key, CancellationToken cancellationToken = default)
	{
		var requestUri = new Uri("api/router/" + HttpUtility.UrlPathEncode(key.ToString()), UriKind.Relative);
		return httpClient.GetFromJsonAsync<DhcpLease>(requestUri, cancellationToken);
	}
}
