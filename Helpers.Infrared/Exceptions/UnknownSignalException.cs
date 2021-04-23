using System;

namespace Helpers.Infrared.Exceptions
{
	public class UnknownSignalException : Exception
	{
		public UnknownSignalException(Models.SignalTypes type)
			: base($"Unknown signal: \"{type:F}\" ")
		{
			Data.Add(nameof(type), type);
		}
	}
}
