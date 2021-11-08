using System;
using System.Threading.Tasks;

namespace Helpers.Discord
{
	public interface IDiscordClient
	{
		Task SendMessageAsync(string message);
	}
}
