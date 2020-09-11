using Helpers.Cineworld.Models;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Helpers.Cineworld.Concrete
{
	public class CineworldClient : Helpers.Web.WebClientBase, ICineworldClient
	{
		private static readonly XmlSerializerFactory _xmlSerializerFactory = new XmlSerializerFactory();

		public CineworldClient(
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(new CineworldHttpClientFactory(), logger, tracer)
		{ }

		public async IAsyncEnumerable<Models.Generated.FilmType> GetFilmsAsync()
		{
			var uri = Settings.AllPerformancesPath;

			var response = await base.SendAsync(HttpMethod.Get, uri);

			await using var stream = await response.TaskStream!;

			var xslt = BuildXslt(Properties.Resources.all_performances);

			var films = Transform<FilmsType>(stream, xslt);

			foreach (var film in films.Film!)
			{
				yield return film;
			}
		}

		public async IAsyncEnumerable<Models.Generated.CinemaType> GetListingsAsync()
		{
			var uri = Settings.ListingsPath;

			var response = await base.SendAsync(HttpMethod.Get, uri);

			await using var stream = await response.TaskStream!;

			var xslt = BuildXslt(Properties.Resources.listings);

			var cinemas = Transform<Models.Generated.CinemasType>(stream, xslt);

			foreach (var cinema in cinemas.Cinema)
			{
				yield return cinema;
			}
		}

		public async Task<DateTime> GetLastModifiedDateAsync()
		{
			var uri = Settings.ListingsPath;

			var response = await base.SendAsync(HttpMethod.Head, uri);

			var values = response.Headers?[Settings.LastModifiedHeaderKey];

			var value = values?.FirstOrDefault() ?? string.Empty;

			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var lastModified))
			{
				return lastModified;
			}

			return DateTime.UtcNow;
		}

		private static T Transform<T>(Stream input, XslCompiledTransform xslt)
		{
			using var reader = XmlReader.Create(input);

			using var output = new MemoryStream();

			using var writer = XmlWriter.Create(output);

			xslt.Transform(reader, writer);

			output.Seek(0L, SeekOrigin.Begin);

			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

			return (T)serializer.Deserialize(output);
		}

		private static XslCompiledTransform BuildXslt(string s)
		{
			using var stringReader = new StringReader(s);
			using var xmlTextReader = new XmlTextReader(stringReader);

			var xslt = new XslCompiledTransform();

			xslt.Load(xmlTextReader);

			return xslt;
		}
	}
}
