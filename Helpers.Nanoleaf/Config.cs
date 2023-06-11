using Microsoft.Extensions.Options;

namespace Helpers.Nanoleaf;

public record Config(Uri BaseAddress, string Token)
	: IOptions<Config>
{
	public Config() : this(default!, default!) { }

	public Config Value => this;
}
