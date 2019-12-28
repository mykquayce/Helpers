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
using System.Xml.Serialization;

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

		public async IAsyncEnumerable<(int edi, string title, short duration)> GetFilmDurationsAsync()
		{
			var uri = Settings.AllPerformancesPath;

			var cinemas = await GetAsync<Helpers.Cineworld.Models.Generated.AllPerformances.cinemasType>(uri);

			foreach (var tuple in from c in cinemas.cinema
								  from f in c.films
								  let tuple = (f.edi, f.title, f.length.ParseLength())
								  group tuple by tuple into gg
								  select gg.Key)
			{
				yield return tuple;
			}
		}

		public async IAsyncEnumerable<Helpers.Cineworld.Models.Generated.Listings.cinemaType> GetListingsAsync()
		{
			var uri = Settings.ListingsPath;

			var cinemas = await GetAsync<Helpers.Cineworld.Models.Generated.Listings.cinemasType>(uri);

			foreach (var cinema in cinemas.cinema)
			{
				yield return cinema;
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

		private async Task<T> GetAsync<T>(Uri uri)
		{
			var (statusCode, stream, headers) = await base.SendAsync(HttpMethod.Get, uri);

			if (statusCode == HttpStatusCode.OK)
			{
				var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));

				var value = (T)serializer.Deserialize(stream);

				return value;
			}

			string response;

			if (stream.CanRead)
			{
				using (stream)
				{
					using var reader = new StreamReader(stream);

					response = await reader.ReadToEndAsync();
				}
			}
			else
			{
				response = string.Empty;
			}

			throw new Exception($"{statusCode:G} response from {uri.OriginalString}")
			{
				Data =
				{
					[nameof(uri)] = uri,
					[nameof(statusCode)] = statusCode,
					[nameof(headers)] = string.Join(';', headers.Select(kvp => $"{kvp.Key}={kvp.Value}")),
					[nameof(response)] = response,
				},
			};
		}
	}
}
