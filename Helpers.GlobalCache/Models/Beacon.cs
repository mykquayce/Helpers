using Dawn;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Helpers.GlobalCache.Models;

public record Beacon(
	string ConfigUrl,
	string Make,
	string Model,
	string PackageLevel,
	string PCB_PN,
	string Revision,
	string SDKClass,
	string Status,
	string Uuid
	)
#if NET7_0_OR_GREATER
	: IParseable<Beacon>
#endif
{
	private const RegexOptions _regexOptions = RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant;
	private readonly static Regex _ipAddressRegex = new(@"^http:\/\/(\d+\.\d+\.\d+\.\d+)$", _regexOptions);
	private readonly static Regex _physicalAddressRegex = new("^GlobalCache_([0-9A-F]{12})$", _regexOptions);

	private IPAddress? _ipAddress;
	private PhysicalAddress? _physicalAddress;

	public IPAddress IPAddress => _ipAddress ??= GetIPAddress();
	public PhysicalAddress PhysicalAddress => _physicalAddress ??= GetPhysicalAddress();

	private IPAddress GetIPAddress()
	{
		var match = _ipAddressRegex.Match(ConfigUrl);
		var s = match.Groups[1].Value;
		return IPAddress.Parse(s);
	}

	public PhysicalAddress GetPhysicalAddress()
	{
		var match = _physicalAddressRegex.Match(Uuid);
		var s = match.Groups[1].Value;
		return PhysicalAddress.Parse(s);
	}

	#region parse
	private const RegexOptions _options = RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant;
	private readonly static Regex _beaconRegex = new("<-(.+?)=(.+?)>", _options);
	private readonly static string[] _keys = { "Config-URL", "Make", "Model", "Pkg_Level", "PCB_PN", "Revision", "SDKClass", "Status", "UUID", };

	public static Beacon Parse(string s, IFormatProvider? provider)
	{
		return TryParse(s, provider, out var result)
			? result
			: throw new ArgumentOutOfRangeException(nameof(s), s, $"unable to parse '{s}' as a {nameof(Beacon)}.");
	}

	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "IParseable<Beacon> implementation")]
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Beacon result)
	{
		Guard.Argument(s!).NotNull().NotEmpty().NotWhiteSpace();

		var d = _beaconRegex.Matches(s!)
			.ToDictionary();

		var values = new List<string>();

		foreach (var key in _keys)
		{
			if (d.TryGetValue(key, out var value))
			{
				values.Add(value!);
			}
			else
			{
				result = default!;
				return false;
			}
		}

		result = new(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8]);
		return true;
	}
	#endregion parse
}
