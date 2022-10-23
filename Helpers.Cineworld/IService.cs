namespace Helpers.Cineworld;

public interface IService
{
	IAsyncEnumerable<Models.Cinema> GetCinemasAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.Film> GetFilmsAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.Show> GetShowsAsync(CancellationToken cancellationToken = default);
}
