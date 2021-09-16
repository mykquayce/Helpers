using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Helpers.Networking.Clients.Concrete
{
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
			Hostname = Guard.Argument(hostname).NotNull().NotEmpty().NotWhiteSpace().Value;
			Port = Guard.Argument(port).Positive().Value;
			NewLine = Guard.Argument(newLine).NotNull().NotEmpty().In("\r", "\n", "\r\n").Value;
		}
		#endregion Constructors

		protected string Hostname { get; }
		protected int Port { get; }
		protected string NewLine { get; }

		public async IAsyncEnumerable<string> SendAndReceiveAsync(string message)
		{
			Guard.Argument(message).NotNull().NotEmpty().NotWhiteSpace();

			using var tcpClient = new System.Net.Sockets.TcpClient(Hostname, Port);
			await using var stream = tcpClient.GetStream();

			await stream.WriteAsync(_encoding.GetBytes(message));
			await stream.FlushAsync();

			if (tcpClient.Client.DontFragment)
			{
				using var reader = new StreamReader(stream, _encoding);

				while (!reader.EndOfStream)
				{
					var line = await reader.ReadLineAsync();
					if (line is not null) yield return line;
				}
			}
			else
			{
				var buffer = new byte[1_024];
				var count = await stream.ReadAsync(buffer);
				var lines = _encoding.GetString(buffer[..count]);
				foreach (var line in lines.Split(NewLine))
				{
					if (line is not null) yield return line;
				}
			}
		}
	}
}
