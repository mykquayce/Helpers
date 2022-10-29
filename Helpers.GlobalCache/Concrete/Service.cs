using Dawn;
using Helpers.GlobalCache.Exceptions;
using Microsoft.Extensions.Options;

namespace Helpers.GlobalCache.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly IReadOnlyDictionary<string, string> _messagesDictionary;

	public Service(IClient client, IOptions<Models.MessagesDictionary> messagesDictionaryOptions)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_messagesDictionary = Guard.Argument(messagesDictionaryOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value.AsReadOnly();
	}

	public async Task SendMessageAsync(string alias, CancellationToken cancellationToken = default)
	{
		var message = _messagesDictionary[alias];
		var response = await _client.SendAsync(message, cancellationToken);
		if (!response.StartsWith("completeir", StringComparison.OrdinalIgnoreCase))
		{
			throw new UnexpectedResponseException(message, response);
		}
	}
}
