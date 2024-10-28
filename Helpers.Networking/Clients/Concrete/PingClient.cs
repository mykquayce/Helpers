using Microsoft.Extensions.Options;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace Helpers.Networking.Clients.Concrete;

public class PingClient : IPingClient
{
	public record Config(int Timeout)
		: IOptions<Config>
	{
		public const int DefaultTimeout = 1_000;

		public Config() : this(DefaultTimeout) { }

		public static Config Defaults => new(DefaultTimeout);

		#region ioptions implementation
		public Config Value => this;
		#endregion ioptions implementation
	}

	private readonly int _timeout;

	public PingClient(IOptions<Config> options)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options?.Value?.Timeout ?? 0);
		_timeout = options!.Value.Timeout;
	}

	public async Task<Models.PacketLossResults> PacketLossTestAsync(IPAddress ip, int milliseconds = 10_000)
	{
		var now = DateTime.UtcNow;
		var results = await PingsAsync(ip, milliseconds).ToListAsync();
		return new(now, results);
	}

	public Task<PingReply> PingAsync(IPAddress ip) => PingsAsync(ip, _timeout).FirstAsync().AsTask();

	public async IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, int milliseconds)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(milliseconds);

		using var cts = new CancellationTokenSource(milliseconds);

		await foreach (var reply in PingsAsync(ip, cts.Token))
		{
			yield return reply;
		}
	}

	public async IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(ip);
		ArgumentNullException.ThrowIfNull(cancellationToken);

		using var ping = new Ping();

		while (!cancellationToken.IsCancellationRequested)
		{
			yield return await ping.SendPingAsync(ip, _timeout);
		}
	}
}
