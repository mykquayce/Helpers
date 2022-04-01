namespace Helpers.GlobalCache;

public interface IClient
{
	IAsyncEnumerable<Models.Beacon> DiscoverAsync();
}
