using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Steam
{
	public interface ISteamClient : IDisposable
	{
		Task<Models.AppDetails> GetAppDetailsAsync(int appId);
		IAsyncEnumerable<Models.Game> GetOwnedGamesAsync(long steamId);
	}
}
