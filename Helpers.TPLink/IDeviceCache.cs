using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink;

public interface IDeviceCache :
	ICollection<Models.Device>,
	IReadOnlyDictionary<string, Models.Device>,
	IReadOnlyDictionary<IPAddress, Models.Device>,
	IReadOnlyDictionary<PhysicalAddress, Models.Device>
{ }
