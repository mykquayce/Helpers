using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class CinemasTypeTests
	{
		private static readonly XmlSerializerFactory _xmlSerializerFactory = new XmlSerializerFactory();

		[Theory]
		[InlineData("Cinemas.xml")]
		public void CinemasTypeTests_Deserialize(string fileName)
		{
			var cinemas = DeserializeFile<Models.Generated.CinemasType>(fileName);

			Assert.NotNull(cinemas);
			Assert.NotNull(cinemas.Cinema);
			Assert.NotEmpty(cinemas.Cinema);

			foreach (var cinema in cinemas.Cinema)
			{
				Assert.NotNull(cinema);
				Assert.InRange(cinema.Id, 1, short.MaxValue);
				Assert.NotNull(cinema.Name);
				Assert.NotEmpty(cinema.Name);
				Assert.NotNull(cinema.Films);
				Assert.NotEmpty(cinema.Films);

				foreach (var film in cinema.Films)
				{
					Assert.NotNull(film);
					Assert.InRange(film.Edi, 0, int.MaxValue);
					Assert.NotNull(film.Title);
					Assert.NotEmpty(film.Title);
					Assert.False(film.Title.StartsWith("A "));
					Assert.False(film.Title.StartsWith("An "));
					Assert.False(film.Title.StartsWith("The "));
					Assert.NotEqual(default, film.Formats);
					Assert.NotNull(film.DateTimes);
					Assert.NotEmpty(film.DateTimes);

					Assert.All(film.DateTimes, dt => Assert.NotEqual(default, dt));
				}
			}
		}

		private static T DeserializeFile<T>(string fileName)
		{
			var path = Path.Combine("Data", fileName);

			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

			return (T)serializer.Deserialize(stream);
		}
	}
}
