using System;
using System.Threading.Tasks;

namespace Helpers.Discord
{
	public interface IDiscordClient : IDisposable
	{
		Task SendMessageAsync(string message);
	}
}
