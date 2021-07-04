using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Concrete
{
	public class DeviceCache : IDeviceCache
	{
		private readonly ICollection<Models.Device> _devices = new List<Models.Device>();

		public Models.Device this[string key] => TryGetValue(key, out var result) ? result : throw new KeyNotFoundException(key + " not found in collection.");
		public Models.Device this[IPAddress key] => TryGetValue(key, out var result) ? result : throw new KeyNotFoundException(key + " not found in collection.");
		public Models.Device this[PhysicalAddress key] => TryGetValue(key, out var result) ? result : throw new KeyNotFoundException(key + " not found in collection.");

		public int Count => _devices.Count;
		public bool IsReadOnly => _devices.IsReadOnly;

		IEnumerable<string> IReadOnlyDictionary<string, Models.Device>.Keys => _devices.Select(d => d.Alias);
		IEnumerable<IPAddress> IReadOnlyDictionary<IPAddress, Models.Device>.Keys => _devices.Select(d => d.IPAddress);
		IEnumerable<PhysicalAddress> IReadOnlyDictionary<PhysicalAddress, Models.Device>.Keys => _devices.Select(d => d.PhysicalAddress);
		public IEnumerable<Models.Device> Values => _devices;

		public void Add(Models.Device item)
		{
			if (!Contains(item)) _devices.Add(item);
		}

		public void Clear() => _devices.Clear();
		public bool Contains(Models.Device item) => _devices.Contains(item);
		public bool ContainsKey(string key) => TryGetValue(key, out var _);
		public bool ContainsKey(IPAddress key) => TryGetValue(key, out var _);
		public bool ContainsKey(PhysicalAddress key) => TryGetValue(key, out var _);
		public void CopyTo(Models.Device[] array, int arrayIndex) => _devices.CopyTo(array, arrayIndex);
		public bool Remove(Models.Device item) => _devices.Remove(item);

		public bool TryGetValue(string key, out Models.Device value) => (value = _devices.SingleOrDefault(d => string.Equals(d.Alias, key, StringComparison.InvariantCultureIgnoreCase))) is not null;
		public bool TryGetValue(IPAddress key, out Models.Device value) => (value = _devices.SingleOrDefault(d => Equals(d.IPAddress, key))) is not null;
		public bool TryGetValue(PhysicalAddress key, out Models.Device value) => (value = _devices.SingleOrDefault(d => Equals(d.PhysicalAddress, key))) is not null;

		public IEnumerator<Models.Device> GetEnumerator() => _devices.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _devices.GetEnumerator();
		IEnumerator<KeyValuePair<string, Models.Device>> IEnumerable<KeyValuePair<string, Models.Device>>.GetEnumerator() => _devices.ToDictionary(d => d.Alias, d => d).GetEnumerator();
		IEnumerator<KeyValuePair<IPAddress, Models.Device>> IEnumerable<KeyValuePair<IPAddress, Models.Device>>.GetEnumerator() => _devices.ToDictionary(d => d.IPAddress, d => d).GetEnumerator();
		IEnumerator<KeyValuePair<PhysicalAddress, Models.Device>> IEnumerable<KeyValuePair<PhysicalAddress, Models.Device>>.GetEnumerator() => _devices.ToDictionary(d => d.PhysicalAddress, d => d).GetEnumerator();
	}
}
