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
		private readonly static Regex _beaconRegex = new Regex("<-(.+?)=(.+?)>", _options);

		public static Beacon Parse(string s)
		{
			var dictionary = _beaconRegex
				.Matches(s)
				.Select(match => (match.Groups[1].Value, match.Groups[2].Value))
				.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2, StringComparer.InvariantCultureIgnoreCase);

			return new Beacon(dictionary);
		}
		#endregion parse
	}
}
