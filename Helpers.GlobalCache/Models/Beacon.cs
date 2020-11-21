using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helpers.GlobalCache.Models
{
	public sealed class Beacon : ReadOnlyDictionary<string, string>
	{
		private Beacon(IDictionary<string, string> dictionary)
			: base(dictionary)
		{ }

		public string ConfigUrl => this["Config-URL"];
		public string Make => this["Make"];
		public string Model => this["Model"];
		public string PackageLevel => this["Pkg_Level"];
		public string PCB_PN => this["PCB_PN"];
		public string Revision => this["Revision"];
		public string SDKClass => this["SDKClass"];
		public string Status => this["Status"];
		public string Uuid => this["UUID"];

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

			return new Beacon(dictionary);
		}
		#endregion parse
	}
}
