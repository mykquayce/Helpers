using Helpers.GlobalCache.Exceptions;
using Microsoft.Extensions.Options;

namespace Helpers.GlobalCache.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly IReadOnlyDictionary<string, string> _messagesDictionary;

	public Service(IClient client, IOptions<Models.MessagesDictionary> messagesDictionaryOptions)
	{
		ArgumentNullException.ThrowIfNull(client);
		ArgumentNullException.ThrowIfNull(messagesDictionaryOptions?.Value?.Value);
		ArgumentOutOfRangeException.ThrowIfZero(messagesDictionaryOptions.Value.Value.Count);
		_client = client;
		_messagesDictionary = messagesDictionaryOptions.Value.Value.AsReadOnly();
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
