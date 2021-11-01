namespace Helpers.MySql;

[Flags]
public enum ExceptionTypes : byte
{
	None = 0,
	TargetMachineActivelyRefused = 1,
	UnknownDatabase = 2,
}
