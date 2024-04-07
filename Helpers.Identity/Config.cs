using Microsoft.Extensions.Options;

namespace Helpers.Identity;

public record Config(Uri Authority, string ClientId, string ClientSecret, string Scope)
	: IOptions<Config>
{
	public Config()
		: this(default!, default!, default!, default!)
	{ }

	public Config Value => this;
}
