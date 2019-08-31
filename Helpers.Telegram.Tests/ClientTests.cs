using System.Threading.Tasks;
using Xunit;

namespace Helpers.Telegram.Tests
{
	public class ClientTests
	{
		[Theory]
		[InlineData(
			-396035426,
			"my sample text")]
		public async Task ClientTests_SendAMessage(
			int chatId,
			string message)
		{
			var apiKey = Helpers.Common.EnvironmentHelpers.GetEnvironmentVariable("TelegramApiKey");

			var messageId = await Client.SendMesssageAsync(apiKey, chatId, message);

			Assert.InRange(messageId, 1, int.MaxValue);
		}
	}
}
