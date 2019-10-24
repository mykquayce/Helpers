using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class DeserializationTests : IDisposable
	{
		private readonly Func<DateTime> _getUtcNow;
		private readonly XmlSerializer _xmlSerializer;

		public DeserializationTests()
		{
			_getUtcNow = ExtensionMethods.GetUtcNow;
			_xmlSerializer = new XmlSerializer(typeof(cinemasType));
		}

		public void Dispose()
		{
			Dispose(managed: true);

			GC.SuppressFinalize(obj: this);
		}

		protected virtual void Dispose(bool managed)
		{
			ExtensionMethods.GetUtcNow = _getUtcNow;
		}

		[Theory]
		[InlineData("all-performances.xml")]
		public void DeserializationTests_Test1(string fileName)
		{
			// Arrange
			ExtensionMethods.GetUtcNow = () => new DateTime(2019, 10, 16, 6, 23, 32, 547, DateTimeKind.Utc);

			// Act
			var cinemas = Deserialize(fileName);

			// Assert
			Assert.NotNull(cinemas);
			Assert.NotNull(cinemas.cinema);
			Assert.NotEmpty(cinemas.cinema);

			var titles = new List<string>();

			foreach (var cinema in cinemas.cinema)
			{
				Assert.NotNull(cinema.name);
				Assert.InRange(cinema.id, 1, short.MaxValue);
				Assert.NotNull(cinema.films);
				Assert.NotEmpty(cinema.films);

				foreach (var film in cinema.films)
				{
					Assert.Matches(@"^(\d{2,3}) mins$", film.length);

					Assert.InRange(film.Duration, 10d, 999d);
					Assert.Equal(film.length, $"{film.Duration:D} mins");

					Assert.NotNull(film.title);
					Assert.InRange(film.edi, 1, int.MaxValue);
					Assert.NotNull(film.shows);
					Assert.NotEmpty(film.shows);

					titles.Add(film.title);

					foreach (var show in film.shows)
					{
						Assert.NotNull(show.date);
						Assert.Matches(@"^(Sun|Mon|Tue|Wed|Thu|Fri|Sat) \d\d (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)$", show.date);
						Assert.NotNull(show.time);
						Assert.Matches(@"^\d\d:\d\d$", show.time);
					}
				}
			}

			var deduped = titles.GroupBy(s => s).Select(g => g.Key).ToList();
		}

		private cinemasType Deserialize(string fileName)
		{
			var path = Path.Combine("Data", fileName);

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			return (cinemasType)_xmlSerializer.Deserialize(stream);
		}
	}
}
