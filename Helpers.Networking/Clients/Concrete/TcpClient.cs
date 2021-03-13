using Dawn;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients.Concrete
{
	public class TcpClient : ITcpClient
	{
		#region Config
		public record Config(string? Hostname, ushort? Port)
		{
			public Config() : this(default, default) { }
		}
		#endregion Config

		private readonly string _hostname;
		private readonly int _port;

		#region Constructors
		public TcpClient(IOptions<Config> options)
			: this(options.Value)
		{ }

		public TcpClient(Config config)
			: this(config.Hostname, config.Port)
		{ }

		public TcpClient(string? hostname, ushort? port)
		{
			_hostname = Guard.Argument(() => hostname!).NotNull().NotEmpty().NotWhiteSpace().Value;
			_port = Guard.Argument(() => port!).NotNull().Value;
		}
		#endregion Constructors

		public async Task<string> SendAndReceiveAsync(string message)
		{
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();

			using var tcpClient = new System.Net.Sockets.TcpClient();
			await tcpClient.ConnectAsync(_hostname, _port);
			await using var stream = tcpClient.GetStream();
			using var reader = new StreamReader(stream);
			using var writer = new StreamWriter(stream)
			{
				NewLine = "\n",
			};

			await writer.WriteLineAsync(message);
			await writer.FlushAsync();
			var response = await reader.ReadToEndAsync();

			return response;
		}
	}
}
