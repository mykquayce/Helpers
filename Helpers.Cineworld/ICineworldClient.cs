using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Cineworld
{
	public interface ICineworldClient : IDisposable
	{
		IAsyncEnumerable<Models.Generated.FilmType> GetFilmsAsync();
		IAsyncEnumerable<Models.Generated.CinemaType> GetListingsAsync();
		Task<DateTime> GetLastModifiedDateAsync();
	}
}
