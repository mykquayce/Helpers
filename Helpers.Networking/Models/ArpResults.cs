using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Helpers.Networking.Models
{
	public record ArpResults(IPAddress IPAddress, IList<ArpResult> Results)
	{
		public static ArpResults Parse(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s));

			var ipAddress = ParseIPAddress(s);
			var results = ParseResults(s).Where(r => r.Type == ArpResult.Types.Dynamic).ToList();

			return new ArpResults(ipAddress, results);
		}

		public static IPAddress ParseIPAddress(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s));

			var match = Regex.Match(s, @"Interface: ((?:\d+\.?){4}) --- 0x\d+");

			if (!match.Success) throw new ArgumentOutOfRangeException(nameof(s), s);

			var ipAddressString = match.Groups[1].Value;

			if (!IPAddress.TryParse(ipAddressString, out var ipAddress))
			{
				throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected {nameof(ipAddressString)}: {ipAddressString}");
			}

			return ipAddress;
		}

		public static IEnumerable<ArpResult> ParseResults(string s)
		{
			var matches = Regex.Matches(s, @"((?:\d+\.?){4})\s+((?:[0-9a-f]{2}-?){6})\s+(dynamic|static)");

			foreach (Match match in matches)
			{
				var ipAddressString = match.Groups[1].Value;
				var physicalAddressString = match.Groups[2].Value;
				var typeString = match.Groups[3].Value;

				if (!IPAddress.TryParse(ipAddressString, out var ipAddress))
				{
					throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected {nameof(ipAddressString)}: {ipAddressString}");
				}

				if (!PhysicalAddress.TryParse(physicalAddressString, out var physicalAddress))
				{
					throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected {nameof(physicalAddressString)}: {physicalAddressString}");
				}

				if (!Enum.TryParse<ArpResult.Types>(typeString, ignoreCase: true, out var type))
				{
					throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected {nameof(typeString)}: {typeString}");
				}

				yield return new ArpResult(physicalAddress, ipAddress, type);
			}
		}
	}
}
