using Dawn;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Helpers.GlobalCache.Extensions
{
	public static class BeaconExtensions
	{
		private const RegexOptions _regexOptions = RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant;

		private const string _ipAddressRegexPattern = @"^http:\/\/(\d+\.\d+\.\d+\.\d+)$";
		private readonly static Regex _ipAddressRegex = new(_ipAddressRegexPattern, _regexOptions);

		private const string _physicalAddressRegexPattern = "^GlobalCache_([0-9A-F]{12})$";
		private readonly static Regex _physicalAddressRegex = new(_physicalAddressRegexPattern, _regexOptions);

		public static IPAddress GetIPAddress(this Models.Beacon beacon)
		{
			var value = Guard.Argument(() => beacon).NotNull().Wrap(b => b.ConfigUrl)
				.NotNull().NotEmpty().Matches(_ipAddressRegexPattern).Value;

			var match = _ipAddressRegex.Match(value);
			var s = match.Groups[1].Value;
			return IPAddress.Parse(s);
		}

		public static PhysicalAddress GetPhysicalAddress(this Models.Beacon beacon)
		{
			var value = Guard.Argument(() => beacon).NotNull().Wrap(b => b.Uuid)
				.NotNull().NotEmpty().Matches(_physicalAddressRegexPattern).Value;

			var match = _physicalAddressRegex.Match(value);
			var s = match.Groups[1].Value;
			return PhysicalAddress.Parse(s);
		}
	}
}
