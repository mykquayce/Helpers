using System;

namespace Helpers.Infrared.Exceptions
{
	public class EmptyResponseException : Exception
	{
		public EmptyResponseException(string host, string message)
			: base($"Empty response from {host} when sending \"{message}\"")
		{
			Data.Add(nameof(host), host);
			Data.Add(nameof(message), message);
		}
	}
}
