using Microsoft.Extensions.Options;

namespace Helpers.Elgato;

public record Config(string Scheme, int Port)
	: IOptions<Config>
{
	public const string DefaultScheme = "http";
	public const int DefaultPort = 9_123;

	public Config() : this(DefaultScheme, DefaultPort) { }

	public static Config Defaults => new();

	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation
}
