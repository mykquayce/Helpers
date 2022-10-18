using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Models;

public record Device(string Alias, IPAddress IPAddress, PhysicalAddress PhysicalAddress);
