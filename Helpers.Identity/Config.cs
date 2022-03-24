using Microsoft.Extensions.Options;

namespace Helpers.Identity;

public record Config(Uri Authority, string ClientId, string ClientSecret, string Scope)
	: IOptions<Config>
{
	public static readonly Uri DefaultAuthority = new("https://identityserver");
	public const string DefaultClientId = nameof(ClientId);
	public const string DefaultClientSecret = nameof(ClientSecret);
	public const string DefaultScope = nameof(Scope);

	public Config()
		: this(DefaultAuthority, DefaultClientId, DefaultClientSecret, DefaultScope)
	{ }

	public Config Value => this;

	public static Config Defaults => new();
}
