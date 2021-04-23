using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Infrared.Clients.Concrete
{
	public class GlobalCacheClient : Clients.IClient
	{
		public record Config(ushort Port)
		{
			public const ushort DefaultPort = 4_998;

			public Config() : this(Port: DefaultPort) { }
		}

		private readonly ushort _port;
		private readonly static Encoding _encoding = Encoding.UTF8;

		#region constructors
		public GlobalCacheClient(IOptions<Config> options) : this(options.Value) { }
		public GlobalCacheClient(Config config) : this(config.Port) { }
		public GlobalCacheClient(ushort port = Config.DefaultPort)
		{
			_port = port;
		}
		#endregion constructors

		public async Task SendAsync(string host, string message)
		{
			var tcpClient = new Helpers.Networking.Clients.Concrete.TcpClient(host, _port, "\r");

			string? response;
			response = await tcpClient.SendAndReceiveAsync(message).FirstAsync();
			processResponse();
			await Task.Delay(millisecondsDelay: 200);
			response = await tcpClient.SendAndReceiveAsync(message).FirstAsync();
			processResponse();

			void processResponse()
			{
				if (string.IsNullOrWhiteSpace(response))
				{
					throw new Exceptions.EmptyResponseException(host, message);
				}

				if (!response.StartsWith("completeir", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exceptions.ErrorResponseException(host, message, response);
				}
			}
		}
	}
}
