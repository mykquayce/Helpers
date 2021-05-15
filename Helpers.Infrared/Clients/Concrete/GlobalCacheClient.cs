using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers.Infrared.Clients.Concrete
{
	public class GlobalCacheClient : Clients.IClient
	{
		public record Config(ushort Port, string NewLine)
		{
			public const ushort DefaultPort = 4_998;
			public const string DefaultNewLine = "\r";

			public Config() : this(Defaults) { }

			public static Config Defaults => new(DefaultPort, DefaultNewLine);
		}

		private readonly string _newLine;
		private readonly ushort _port;

		#region constructors
		public GlobalCacheClient(IOptions<Config> options) : this(options.Value) { }
		public GlobalCacheClient(Config config) : this(config.Port, config.NewLine) { }
		public GlobalCacheClient(ushort port, string newLine)
		{
			_newLine = Guard.Argument(() => newLine).NotNull().NotEmpty().In("\r", "\n", "\r\n").Value;
			_port = Guard.Argument(() => port).Positive().Value;
		}
		#endregion constructors

		public async Task SendAsync(string host, string message)
		{
			var tcpClient = new Helpers.Networking.Clients.Concrete.TcpClient(host, _port, _newLine);

			var response = await tcpClient.SendAndReceiveAsync(message).FirstAsync();
			if (string.IsNullOrWhiteSpace(response)) throw new Exceptions.EmptyResponseException(host, message);
			if (!response.StartsWith("completeir", StringComparison.InvariantCultureIgnoreCase)) throw new Exceptions.ErrorResponseException(host, message, response);

		}
	}
}
