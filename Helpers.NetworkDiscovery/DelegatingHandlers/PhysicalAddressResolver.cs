using System.Net.NetworkInformation;

namespace System.Net.Http;

public class PhysicalAddressResolver(Helpers.NetworkDiscovery.IClient client) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var host = request.RequestUri?.Host;

		if (!string.IsNullOrEmpty(host)
			&& (host.Length == 12 || host.Length == 17)
			&& PhysicalAddress.TryParse(host, out var mac))
		{
			var lease = await client.ResolveAsync(mac, cancellationToken);

			var builder = new UriBuilder(request.RequestUri!)
			{
				Host = lease.IPAddress.ToString(),
			};

			request.RequestUri = builder.Uri;
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
