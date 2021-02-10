using Helpers.Networking.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Helpers.Networking.Models
{
	public record ArpResult(PhysicalAddress PhysicalAddress, IPAddress IPAddress, ArpResult.Types Type)
	{
		private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
		private static readonly Regex _regex = new (
			@"(\d+\.\d+\.\d+\.\d+)\s+([0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2})\s+(dynamic|static)",
			_regexOptions);

		[Flags]
		public enum Types
		{
			None = 0,
			Dynamic = 1,
			Static = 2,
		}

		public static IEnumerable<ArpResult> Parse(string s)
		{
			var matches = _regex.Matches(s);

			foreach (Match match in matches)
			{
				var ipAddress = match.Groups[1].Value.ParseIPAddress();
				var physicalAddress = match.Groups[2].Value.ParsePhysicalAddress();
				var typeString = match.Groups[3].Value;

				if (!Enum.TryParse<ArpResult.Types>(typeString, ignoreCase: true, out var type))
				{
					throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected {nameof(typeString)}: {typeString}");
				}

				yield return new ArpResult(physicalAddress, ipAddress, type);
			}
		}
	}
}
