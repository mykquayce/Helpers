namespace Helpers.Cineworld;

public interface IService
{
	IAsyncEnumerable<Models.Cinema> GetCinemasAsync();
	IAsyncEnumerable<Models.Film> GetFilmsAsync();
	IAsyncEnumerator<Models.Show> GetShowsAsync();
}
