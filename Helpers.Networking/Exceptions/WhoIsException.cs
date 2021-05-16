using System;

namespace Helpers.Networking.Exceptions
{
	public class WhoIsException : Exception
	{
		public WhoIsException(string message, string hostName, int port, string error)
			: base($@"Error sending ""{message}"" to {hostName}:{port:D}: {error}")
		{
			Data.Add(nameof(message), message);
			Data.Add(nameof(hostName), hostName);
			Data.Add(nameof(port), port);
			Data.Add(nameof(error), error);
		}
		public WhoIsException(string error)
			: base(error)
		{
			Data.Add(nameof(error), error);
		}
	}
}
