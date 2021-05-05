using Dawn;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.SSH.Clients.Concrete
{
	public class SSHClient : IDisposable, ISSHClient
	{
		private readonly Renci.SshNet.SshClient _sshClient;

		#region constructors
		public SSHClient(string host, int port, string username) => _sshClient = new(host, port, username);
		public SSHClient(string host, int port, string username, string password) => _sshClient = new(host, port, username, password);

		public SSHClient(string host, int port, string username, FileInfo privateKeyFileInfo)
		{
			using var stream = privateKeyFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			using var privateKeyFile = new Renci.SshNet.PrivateKeyFile(stream);
			_sshClient = new(host, port, username, privateKeyFile);
		}
		#endregion constructors

		public async Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000)
		{
			Guard.Argument(() => commandText).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => millisecondsTimeout).Positive();

			if (!_sshClient.IsConnected) _sshClient.Connect();

			using var command = _sshClient.CreateCommand(commandText, Encoding.UTF8);

			command.CommandTimeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

			return await Task.Factory.FromAsync(
				beginMethod: (callback, state) => command.BeginExecute(callback, state),
				endMethod: result => command.EndExecute(result),
				state: commandText);
		}

		#region dispose
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_sshClient?.IsConnected ?? false) _sshClient.Disconnect();
					_sshClient?.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion dispose
	}
}
