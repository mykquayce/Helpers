using System;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Services
{
	public interface IGlobalCacheService : IDisposable
	{
		Task<string> ConnectSendReceiveAsync(string hostName, string message);
	}
}
