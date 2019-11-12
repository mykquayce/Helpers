using System;

namespace Helpers.Cineworld.Models
{
	public class CinemaMovieShow
	{
		public string? Cinema { get; set; }
		public DateTime DateTime { get; set; }
		public TimeSpan End { get; set; }
		public string? Movie { get; set; }
	}
}
