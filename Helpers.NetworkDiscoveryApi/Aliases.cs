using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscoveryApi;

public class Aliases : Dictionary<string, string>, IOptions<Aliases>
{
	public Aliases()
		: base(StringComparer.OrdinalIgnoreCase)
	{ }

	public Aliases(IDictionary<string, string> aliases)
		: base(aliases, StringComparer.OrdinalIgnoreCase)
	{ }

	public static Aliases Defaults => new();

	public Aliases Value => this;
}
