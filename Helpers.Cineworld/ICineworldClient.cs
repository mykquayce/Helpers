using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Cineworld
{
	public interface ICineworldClient : IDisposable
	{
		IAsyncEnumerable<Models.Generated.FilmType> GetFilmDurationsAsync();
		IAsyncEnumerable<Models.Generated.CinemaType> GetListingsAsync();
		Task<DateTime> GetListingsLastModifiedDateAsync();
	}
}
