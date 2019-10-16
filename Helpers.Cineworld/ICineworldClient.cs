using System;
using System.Threading.Tasks;

namespace Helpers.Cineworld
{
	public interface ICineworldClient : IDisposable
	{
		Task<Models.cinemasType> GetPerformancesAsync();
		Task<DateTime> GetPerformancesLastModifiedDateAsync();
	}
}
