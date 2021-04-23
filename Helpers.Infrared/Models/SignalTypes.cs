using System;

namespace Helpers.Infrared.Models
{
	[Flags]
	public enum SignalTypes : byte
	{
		ToggleMute = 1,
		TogglePower = 2,
		VolumeDown = 4,
		VolumeUp = 8,
	}
}
