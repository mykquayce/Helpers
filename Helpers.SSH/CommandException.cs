namespace Helpers.SSH;

public class CommandException : Exception
{
	public CommandException(Renci.SshNet.SshCommand command)
		: base($"{command.CommandText} returned {command.Error}")
	{
		Data.Add(nameof(command.CommandText), command.CommandText);
		Data.Add(nameof(command.Error), command.Error);
		Data.Add(nameof(command.Result), command.Result);
		Data.Add(nameof(command.ExitStatus), command.ExitStatus);
	}
}
