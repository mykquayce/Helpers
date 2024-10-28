using System.Text;

namespace Helpers.SSH.Concrete;

public class Client(Renci.SshNet.SshClient @base) : IClient
{
	public async Task<string> RunCommandAsync(string commandText, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrEmpty(commandText);

		if (!@base.IsConnected) { await @base.ConnectAsync(cancellationToken); }

		using var command = @base.CreateCommand(commandText, Encoding.UTF8);
		await command.ExecuteAsync(cancellationToken);
		return command.Result;
	}
}
