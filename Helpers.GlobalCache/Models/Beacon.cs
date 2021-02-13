using Dawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helpers.GlobalCache.Models
{
	public record Beacon(
		string? ConfigUrl,
		string? Make,
		string? Model,
		string? PackageLevel,
		string? PCB_PN,
		string? Revision,
		string? SDKClass,
		string? Status,
		string? Uuid
		)
	{
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

			var keysString = string.Join(',', keys!);

			string f(string key) => dictionary.TryGetValue(key, out var s)
				? s
				: throw new KeyNotFoundException($"{key} {nameof(key)} not found in {nameof(keys)}: {keysString}");

			var configUrl = f("Config-URL");
			var make = f("Make");
			var model = f("Model");
			var pkgLevel = f("Pkg_Level");
			var pcb_pn = f("PCB_PN");
			var revision = f("Revision");
			var sdkClass = f("SDKClass");
			var status = f("Status");
			var uuid = f("UUID");

			return new Beacon(configUrl, make, model, pkgLevel, pcb_pn, revision, sdkClass, status, uuid);
		}
		#endregion parse
	}
}
