using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue;

public record Config(string Hostname, string Username)
		: IOptions<Config>
{
	public const string DefaultHostname = "192.168.1.157";
	public const string DefaultUsername = "i35sdUz4iZI0XPWxbIdQKdp76t4cH8LOwUCtFcFJ";

	public Config() : this(DefaultHostname, DefaultUsername) { }

	public static Config Defaults => new();

	public Config Value => this;
}
