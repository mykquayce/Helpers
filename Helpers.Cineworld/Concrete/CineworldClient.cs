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
	public class CineworldClient : Helpers.HttpClient.HttpClientBase, ICineworldClient
	{
		private static readonly XmlSerializerFactory _xmlSerializerFactory = new XmlSerializerFactory();

		public CineworldClient(
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(new CineworldHttpClientFactory(), logger, tracer)
		{ }

		public async IAsyncEnumerable<Models.Generated.FilmType> GetFilmDurationsAsync()
		{
			var uri = Settings.AllPerformancesPath;

			var (_, stream, _) = await base.SendAsync(HttpMethod.Get, uri);

			using (stream)
			{
				var xslt = BuildXslt(Properties.Resources.all_performances);

				var films = Transform<Models.Generated.FilmType[]>(stream, xslt);

				foreach (var film in films)
				{
					yield return film;
				}
			}
		}

		public async IAsyncEnumerable<Models.Generated.CinemaType> GetListingsAsync()
		{
			var uri = Settings.ListingsPath;

			var (_, stream, _) = await base.SendAsync(HttpMethod.Get, uri);

			using (stream)
			{
				var xslt = BuildXslt(Properties.Resources.listings);

				var cinemas = Transform<Models.Generated.CinemasType>(stream, xslt);

				foreach (var cinema in cinemas.Cinema)
				{
					yield return cinema;
				}
			}
		}

		public async Task<DateTime> GetListingsLastModifiedDateAsync()
		{
			var uri = Settings.ListingsPath;

			var (_, _, headers) = await base.SendAsync(HttpMethod.Head, uri);

			var s = headers?[Settings.LastModifiedHeaderKey]?.FirstOrDefault() ?? string.Empty;

			if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var lastModified))
			{
				return lastModified;
			}

			return DateTime.UtcNow;
		}

		private static T Transform<T>(Stream input, XslTransform xslt)
		{
			var doc = new XPathDocument(input);

			using var output = new MemoryStream();

			xslt.Transform(doc, args: default, output);

			output.Seek(0L, SeekOrigin.Begin);

			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

			return (T)serializer.Deserialize(output);
		}

		private static XslTransform BuildXslt(string s)
		{
			using var stringReader = new StringReader(s);
			using var xmlTextReader = new XmlTextReader(stringReader);

			var xslt = new XslTransform();

			xslt.Load(xmlTextReader);

			return xslt;
		}
	}
}
