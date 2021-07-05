using System;
using System.Collections.Generic;

namespace Helpers.GlobalCache
{
	public interface IClient
	{
		IAsyncEnumerable<Models.Beacon> DiscoverAsync();
	}
}
