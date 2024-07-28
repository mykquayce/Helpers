namespace Helpers.SSH;

public interface IConfig
{
	string Host { get; set; }
	ushort Port { get; set; }
	string Username { get; set; }
	string? Password { get; set; }
	string? PathToPrivateKey { get; set; }
}
