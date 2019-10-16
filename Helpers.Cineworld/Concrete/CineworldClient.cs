using Helpers.Cineworld.Models;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Helpers.Cineworld.Concrete
{
	public class CineworldClient : Helpers.HttpClient.HttpClientBase, ICineworldClient
	{
		private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(cinemasType));

		public CineworldClient(
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(new CineworldHttpClientFactory(), logger, tracer)
		{ }

		public async Task<cinemasType> GetPerformancesAsync()
		{
			var (statusCode, stream, headers) = await base.SendAsync(HttpMethod.Get, Settings.Path);

			if (statusCode == HttpStatusCode.OK)
			{
				using (stream)
				{
					return (cinemasType)_xmlSerializer.Deserialize(stream);
				}
			}

			await (stream?.DisposeAsync() ?? default);

			throw new Exception($"Request for performances returned {statusCode:G}")
			{
				Data =
				{
					[nameof(statusCode)] = statusCode,
					[nameof(headers)] = headers,
				},
			};
		}

		public async Task<DateTime> GetPerformancesLastModifiedDateAsync()
		{
			var (statusCode, stream, headers) = await base.SendAsync(HttpMethod.Head, Settings.Path);

			await (stream?.DisposeAsync() ?? default);

			if (statusCode != HttpStatusCode.OK)
			{
				throw new Exception($"Request for performances last-modified date returned {statusCode:G}")
				{
					Data =
					{
						[nameof(statusCode)] = statusCode,
						[nameof(headers)] = headers,
					},
				};
			}

			try
			{
				var s = headers?[Settings.LastModifiedHeaderKey]?.FirstOrDefault();

				return DateTime.Parse(s, styles: DateTimeStyles.AdjustToUniversal);
			}
			catch (Exception ex)
			{
				throw new Exception("Request for performances last-modified date returned " + ex.Message, ex)
				{
					Data =
					{
						[nameof(statusCode)] = statusCode,
						[nameof(headers)] = headers,
					},
				};
			}
		}
	}
}
