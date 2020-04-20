using Helpers.Steam.Models;
using System;
using System.Collections.Generic;

namespace Helpers.Steam
{
	public interface ISteamClient : IDisposable
	{
		IAsyncEnumerable<Game> GetOwnedGamesAsync(long steamId);
	}
}
