namespace Helpers.GlobalCache;

public interface IService
{
	Task SendMessageAsync(string alias, CancellationToken cancellationToken = default);
}
