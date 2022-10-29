using System.Text;
using System.Text.Json;

namespace Helpers.GlobalCache;

public interface IClient : IAsyncDisposable
{
	private static readonly Encoding _encoding = Encoding.UTF8;

	Task<ReadOnlyMemory<byte>> SendAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default);

	async Task<string> SendAsync(string message, CancellationToken cancellationToken = default)
	{
		var bytes = _encoding.GetBytes(message);
		var response = await SendAsync(bytes, cancellationToken);
		return _encoding.GetString(response.ToArray());
	}

	async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest message, CancellationToken cancellationToken = default)
	{
		var messageJson = JsonSerializer.Serialize<TRequest>(message);
		var responseJson = await SendAsync(messageJson, cancellationToken);
		return JsonSerializer.Deserialize<TResponse>(responseJson)!;
	}
}
