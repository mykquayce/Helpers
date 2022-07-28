using Dawn;
using Microsoft.Extensions.Options;
using System.Text;

namespace Helpers.SSH.Concrete;

public class Client : IClient
{
	private readonly Renci.SshNet.SshClient _sshClient;
	private static readonly Encoding _encoding = Encoding.UTF8;
	private static readonly string _terminalName = string.Empty;
	private const uint _columns = 80, _rows = 40, _width = 800, _height = 600;
	private const int _bufferSize = 1_024;

	public Client(IOptions<Config> options)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;

		if (!string.IsNullOrWhiteSpace(config.Password))
		{
			_sshClient = new(config.Host, config.Port, config.Username, config.Password);
		}
		else if (!string.IsNullOrWhiteSpace(config.PathToPrivateKey))
		{
			var path = FixPath(config.PathToPrivateKey);
			var privateKeyFileInfo = new FileInfo(path);
			using var stream = privateKeyFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			using var privateKeyFile = new Renci.SshNet.PrivateKeyFile(stream);
			_sshClient = new(config.Host, config.Port, config.Username, privateKeyFile);
		}
		else
		{
			_sshClient = new(config.Host, config.Port, config.Username);
		}
	}

	public static string FixPath(string path)
	{
		Guard.Argument(path).NotNull().NotEmpty().NotWhiteSpace();

		if (path[0] != '~')
		{
			return path;
		}

		var pwd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify);
		var paths = (pwd + path[1..]).Split('/', '\\');
		return Path.Combine(paths);
	}

	public async Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000)
	{
		Guard.Argument(commandText).NotNull().NotEmpty().NotWhiteSpace();
		Guard.Argument(millisecondsTimeout).Positive();

		if (!_sshClient.IsConnected) _sshClient.Connect();

		using var command = _sshClient.CreateCommand(commandText, _encoding);

		command.CommandTimeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

		return await Task.Factory.FromAsync(
			beginMethod: (callback, state) => command.BeginExecute(callback, state),
			endMethod: result => command.EndExecute(result),
			state: commandText);
	}

	public async IAsyncEnumerable<string> RunCommandAsShellAsync(string commandText, CancellationToken? cancellationToken = null)
	{
		Guard.Argument(commandText).NotNull().NotEmpty().NotWhiteSpace();

		if (!_sshClient.IsConnected) _sshClient.Connect();

		using var shellStream = _sshClient.CreateShellStream(_terminalName, _columns, _rows, _width, _height, _bufferSize);

		await Task.Delay(millisecondsDelay: 10, cancellationToken ?? CancellationToken.None);

		var prompt = await shellStream.ReadLinesAsync(cancellationToken ?? CancellationToken.None)
			.LastAsync(cancellationToken ?? CancellationToken.None);

		shellStream.WriteLine(commandText);

		await Task.Delay(millisecondsDelay: 100, cancellationToken ?? CancellationToken.None);

		var lines = shellStream
			.ReadLinesAsync(cancellationToken ?? CancellationToken.None)
			.Skip(1)
			.TakeWhile(s => s != prompt);

		await foreach (var line in lines)
		{
			yield return line;
		}
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
