using System;
using System.Collections.Generic;
using System.Net;

namespace Helpers.Networking.Clients
{
	public interface IWhoIsClient : IDisposable
	{
		IAsyncEnumerable<Models.SubnetAddress> GetIpsAsync(int asn);
	}
}
