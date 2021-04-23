using System;

namespace Helpers.Infrared.Exceptions
{
	public class UnknownDeviceException : Exception
	{
		public UnknownDeviceException(string alias)
			: base($"Unknown device: \"{alias}\" ")
		{
			Data.Add(nameof(alias), alias);
		}
	}
}
