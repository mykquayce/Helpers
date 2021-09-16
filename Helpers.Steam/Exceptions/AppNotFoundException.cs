using Dawn;
using System.Collections.Generic;

namespace Helpers.Steam.Exceptions
{
	public class AppNotFoundException : KeyNotFoundException
	{
		public AppNotFoundException(int appId)
			: base($"App {appId:D} not found")
		{
			AppId = Guard.Argument(appId).Positive().Value;

			Data.Add(nameof(appId), appId);
		}

		public int AppId { get; }
	}
}
