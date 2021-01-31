using Dawn;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients.Concrete
{
	public class TcpClient : ITcpClient
	{
		private readonly string _hostname;
		private readonly int _port;
		private readonly System.Net.Sockets.TcpClient _tcpClient;
		private bool _isDisposed;

		public TcpClient(string hostname, int port)
		{
			_hostname = Guard.Argument(() => hostname).NotNull().NotEmpty().NotWhiteSpace().Value;
			_port = Guard.Argument(() => port).InRange(0, 65_535).Value;
			_tcpClient = new System.Net.Sockets.TcpClient();
		}

		public async Task<string> SendAndReceiveAsync(string message)
		{
			await _tcpClient.ConnectAsync(_hostname, _port);
			await using var stream = _tcpClient.GetStream();
			using var reader = new StreamReader(stream);
			using var writer = new StreamWriter(stream)
			{
				NewLine = "\r\n",
			};

			await writer.WriteLineAsync(message);
			await writer.FlushAsync();
			var response = await reader.ReadToEndAsync();

			return response;
		}

		#region disposing
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing) _tcpClient?.Dispose();
				_isDisposed = true;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion disposing
	}
}
