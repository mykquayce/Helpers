using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Cineworld
{
	public interface ICineworldClient : IDisposable
	{
		IAsyncEnumerable<(int edi, string title, short duration)> GetFilmDurationsAsync();
		IAsyncEnumerable<Models.Generated.Listings.cinemaType> GetListingsAsync();
		Task<DateTime> GetListingsLastModifiedDateAsync();
	}
}
