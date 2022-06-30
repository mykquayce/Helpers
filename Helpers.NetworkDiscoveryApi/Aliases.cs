using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi;

public class Aliases : Dictionary<string, PhysicalAddress>, IOptions<Aliases>
{
	public Aliases()
		: base(StringComparer.OrdinalIgnoreCase)
	{ }

	public Aliases(IDictionary<string, PhysicalAddress> aliases)
		: base(aliases, StringComparer.OrdinalIgnoreCase)
	{ }

	public Aliases(IConfiguration configuration)
		: this(Bind(configuration))
	{ }

	public static Aliases Defaults => new();

	public Aliases Value => this;

	public static Aliases Bind(IConfiguration configuration)
	{
		var stringStringDictionary = new Dictionary<string, string>();
		configuration.Bind(stringStringDictionary);

		var dictionary = new Dictionary<string, PhysicalAddress>();

		foreach (var (alias, physicalAddressString) in stringStringDictionary)
		{
			var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
			dictionary.Add(alias, physicalAddress);
		}

		return new(dictionary);
	}
}
