using Helpers.Networking.Models.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Helpers.Networking.Models
{
	public record ArpResultsDictionary(IDictionary<IPAddress, ICollection<ArpResult>> Dictionary) : IReadOnlyDictionary<IPAddress, ICollection<ArpResult>>
	{
		#region IReadOnlyDictionary implementation
		public ICollection<ArpResult> this[IPAddress key] => Dictionary[key];
		public IEnumerable<IPAddress> Keys => Dictionary.Keys;
		public IEnumerable<ICollection<ArpResult>> Values => Dictionary.Values;
		public int Count => Dictionary.Count;
		public bool ContainsKey(IPAddress key) => Dictionary.ContainsKey(key);
		public IEnumerator<KeyValuePair<IPAddress, ICollection<ArpResult>>> GetEnumerator() => Dictionary.GetEnumerator();
		public bool TryGetValue(IPAddress key, [MaybeNullWhen(false)] out ICollection<ArpResult> value) => Dictionary.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
		#endregion IReadOnlyDictionary implementation

		public static ArpResultsDictionary Parse(string s)
		{
			var kvps = ToKeyValuePairs(s);
			var dictionary = new Dictionary<IPAddress, ICollection<ArpResult>>(kvps);
			var arpResultsDictionary = new ArpResultsDictionary(dictionary);
			return arpResultsDictionary;
		}

		private static IEnumerable<KeyValuePair<IPAddress, ICollection<ArpResult>>> ToKeyValuePairs(string s)
		{
			var sections = s.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

			foreach (var section in sections)
			{
				var match = Regex.Match(section, @"Interface: (\d+\.\d+\.\d+\.\d+) --- 0x[0-9a-f]+");
				var ipAddress = match.Groups[1].Value.ParseIPAddress();
				var arpResults = ArpResult.Parse(section).ToList();
				yield return new KeyValuePair<IPAddress, ICollection<ArpResult>>(ipAddress, arpResults);
			}
		}
	}
}
