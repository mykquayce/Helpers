using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text;

namespace Helpers.Networking.Clients.Concrete;

public class TcpClient : ITcpClient
{
	private readonly static Encoding _encoding = Encoding.UTF8;

	#region Config
	public record Config(string Hostname, ushort Port, string NewLine)
	{
		public const string DefaultHostName = "localhost";
		public const ushort DefaultPort = 43;
		public const string DefaultNewLine = "\n";

		public Config() : this(DefaultHostName, DefaultPort, DefaultNewLine) { }
	}
	#endregion Config

	#region Constructors
	public TcpClient(IOptions<Config> options) : this(options.Value) { }
	public TcpClient(Config config) : this(config.Hostname, config.Port, config.NewLine) { }

	public TcpClient(string hostname, ushort port, string newLine)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(hostname);
		ArgumentOutOfRangeException.ThrowIfZero(port);
		ArgumentException.ThrowIfNullOrEmpty(newLine);
		Hostname = hostname;
		Port = port;
		NewLine = newLine;
	}
	#endregion Constructors

	protected string Hostname { get; }
	protected int Port { get; }
	protected string NewLine { get; }

	public async IAsyncEnumerable<string> SendAndReceiveAsync(string message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(message);

		using var tcpClient = new System.Net.Sockets.TcpClient(Hostname, Port);
		await using var stream = tcpClient.GetStream();

		await stream.WriteAsync(_encoding.GetBytes(message), cancellationToken);
		await stream.FlushAsync(cancellationToken);

		using var reader = new StreamReader(stream, _encoding);

		while (!reader.EndOfStream
			&& !cancellationToken.IsCancellationRequested)
		{
			var line = await reader.ReadLineAsync(cancellationToken);
			if (line is not null) yield return line;
		}
	}
}
