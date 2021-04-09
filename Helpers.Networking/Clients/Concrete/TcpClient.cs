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
		public record Config(string? Hostname, ushort? Port)
		{
			public Config() : this(default, default) { }
		}
		#endregion Config

		#region Constructors
		public TcpClient(IOptions<Config> options)
			: this(options.Value)
		{ }

		public TcpClient(Config config)
			: this(config.Hostname, config.Port)
		{ }

		public TcpClient(string? hostname, ushort? port)
		{
			Hostname = Guard.Argument(() => hostname!).NotNull().NotEmpty().NotWhiteSpace().Value;
			Port = Guard.Argument(() => port!).NotNull().Value;
		}
		#endregion Constructors

		protected string Hostname { get; }
		protected int Port { get; }

		public async IAsyncEnumerable<string> SendAndReceiveAsync(string message)
		{
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();

			using var tcpClient = new System.Net.Sockets.TcpClient(Hostname, Port);
			await using var stream = tcpClient.GetStream();

			await stream.WriteAsync(_encoding.GetBytes(message));
			await stream.FlushAsync();

			using var reader = new StreamReader(stream);

			string? line;

			while ((line = await reader.ReadLineAsync()) is not null)
			{
				yield return line;
			}
		}
	}
}
