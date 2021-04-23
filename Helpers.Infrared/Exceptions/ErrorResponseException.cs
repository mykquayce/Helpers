using System;

namespace Helpers.Infrared.Exceptions
{
	public class ErrorResponseException : Exception
	{
		public ErrorResponseException(string host, string message, string response)
			: base($"Error response \"{response}\" from {host} when sending \"{message}\"")
		{
			Data.Add(nameof(host), host);
			Data.Add(nameof(message), message);
			Data.Add(nameof(response), response);
		}
	}
}
