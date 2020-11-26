using Dawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helpers.GlobalCache.Models
{
	public record Beacon
	{
		public string? ConfigUrl { get; init; }
		public string? Make { get; init; }
		public string? Model { get; init; }
		public string? PackageLevel { get; init; }
		public string? PCB_PN { get; init; }
		public string? Revision { get; init; }
		public string? SDKClass { get; init; }
		public string? Status { get; init; }
		public string? Uuid { get; init; }

		#region parse
		private const RegexOptions _options = RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant;
		private readonly static Regex _beaconRegex = new("<-(.+?)=(.+?)>", _options);

		public static Beacon Parse(string s)
		{
			var matches = _beaconRegex.Matches(s);

			var dictionary = (from m in matches
							  where m.Success
							  let key = m.Groups[1].Value
							  let value = m.Groups[2].Value
							  let kvp = new { key, value, }
							  select kvp
							).ToDictionary(o => o.key, o => o.value, StringComparer.InvariantCultureIgnoreCase);

			return Parse(dictionary);
		}

		public static Beacon Parse(IDictionary<string, string> dictionary)
		{
			var keys = Guard.Argument(() => dictionary).NotNull()
				.Wrap(d => d.Keys).NotNull().NotEmpty().Value;

			string f(string key) => dictionary.TryGetValue(key, out var s)
				? s
				: throw new KeyNotFoundException($"{key} key not found in keys: " + string.Join(",", keys!));

			return new Beacon
			{
				ConfigUrl = f("Config-URL"),
				Make = f("Make"),
				Model = f("Model"),
				PackageLevel = f("Pkg_Level"),
				PCB_PN = f("PCB_PN"),
				Revision = f("Revision"),
				SDKClass = f("SDKClass"),
				Status = f("Status"),
				Uuid = f("UUID"),
			};
		}
		#endregion parse
	}
}
