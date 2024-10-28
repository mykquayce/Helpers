namespace Helpers.Steam.Exceptions
{
	public class AppNotFoundException : KeyNotFoundException
	{
		public AppNotFoundException(int appId)
			: base($"App {appId:D} not found")
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(appId);
			AppId = appId;

			Data.Add(nameof(appId), appId);
		}

		public int AppId { get; }
	}
}
