using System.Text;

namespace Helpers.SSH.Concrete;

public class Client(Renci.SshNet.SshClient @base) : IClient
{
	private static readonly TimeSpan _timeOut = TimeSpan.FromSeconds(5);

	public async Task<string> RunCommandAsync(string commandText, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrEmpty(commandText);

		if (!@base.IsConnected) { await @base.ConnectAsync(cancellationToken); }

		using var command = @base.CreateCommand(commandText, Encoding.UTF8);
		command.CommandTimeout = _timeOut;

		return await Task.Factory.FromAsync(
			beginMethod: (callback, state) => command.BeginExecute(callback, state),
			endMethod: result => command.EndExecute(result),
			state: commandText);
	}
}
