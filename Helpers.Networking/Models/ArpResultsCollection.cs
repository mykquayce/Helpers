using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models
{
	public record ArpResultsCollection(IList<ArpResults> Collection) : IReadOnlyCollection<ArpResults>
	{
		#region IReadOnlyCollection implementation
		public int Count => Collection.Count;
		public IEnumerator<ArpResults> GetEnumerator() => Collection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();
		#endregion IReadOnlyCollection implementation

		private IEnumerable<ArpResult> Results => Collection.SelectMany(r => r.Results);

		private IReadOnlyDictionary<PhysicalAddress, IPAddress> PhysicalAddressDictionary
			=> Results.ToDictionary(r => r.PhysicalAddress, r => r.IPAddress);

		private IReadOnlyDictionary<IPAddress, PhysicalAddress> IPAddressDictionary
			=> Results.ToDictionary(r => r.IPAddress, r => r.PhysicalAddress);

		public IPAddress GetIPAddressFromPhysicalAddress(PhysicalAddress physicalAddress)
		{
			return PhysicalAddressDictionary.TryGetValue(physicalAddress, out var value)
				? value
				: throw new KeyNotFoundException(nameof(value) + " not found");
		}

		public PhysicalAddress GePhysicalAddresstFromIPAddress(IPAddress ipAddress)
		{
			return IPAddressDictionary.TryGetValue(ipAddress, out var value)
				? value
				: throw new KeyNotFoundException(nameof(value) + " not found");
		}

		public static ArpResultsCollection Parse(string s)
		{
			var sections = s.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			var collection = sections.Select(ArpResults.Parse).Where(r => r.Results.Any()).ToList();
			return new ArpResultsCollection(collection);
		}
	}
}
