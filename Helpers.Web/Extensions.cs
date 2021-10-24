using Helpers.Web.Models;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Helpers.Web
{
	public static class ResponseExtensions
	{
		public static void Deconstruct(
			this IResponseBase response,
			out IReadOnlyDictionary<string, StringValues> headers, out HttpStatusCode statusCode)
		{
			if (response is null) throw new ArgumentNullException(nameof(response));

			headers = response.Headers;
			statusCode = response.StatusCode;
		}

		public static void Deconstruct(
			this IResponse response,
			out IReadOnlyDictionary<string, StringValues> headers, out HttpStatusCode statusCode, out Task<Stream> taskStream)
		{
			if (response is null) throw new ArgumentNullException(nameof(response));

			Deconstruct(response, out headers, out statusCode);
			taskStream = response.TaskStream;
		}

		public static void Deconstruct<T>(
			this IResponse<T> response,
			out IReadOnlyDictionary<string, StringValues> headers, out HttpStatusCode statusCode, out T @object)
			where T : class
		{
			if (response is null) throw new ArgumentNullException(nameof(response));

			Deconstruct(response, out headers, out statusCode);
			@object = response.Object;
		}
	}
}
