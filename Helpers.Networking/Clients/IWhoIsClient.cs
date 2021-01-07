using System.Collections.Generic;
using System.Net;

namespace Helpers.Networking.Clients
{
	public interface IWhoIsClient
	{
		IAsyncEnumerable<(IPAddress, byte)> GetIpsAsync(int asn);
	}
}
