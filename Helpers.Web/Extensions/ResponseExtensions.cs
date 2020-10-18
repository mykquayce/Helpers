using Helpers.Web.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.Web.Extensions
{
	public static class ResponseExtensions
	{
		public static void Deconstruct(
			this IResponse response,
			out IReadOnlyDictionary<string, StringValues>? headers, out HttpStatusCode? statusCode, out Task<Stream>? taskStream)
		{
			if (response is null) throw new ArgumentNullException(nameof(response));

			headers = response.Headers;
			statusCode = response.StatusCode;
			taskStream = response.TaskStream;
		}
		public static void Deconstruct<T>(
			this IResponse<T> response,
			out IReadOnlyDictionary<string, StringValues>? headers, out HttpStatusCode? statusCode, out Task<Stream>? taskStream, out T? @object)
			where T : class
		{
			if (response is null) throw new ArgumentNullException(nameof(response));

			headers = response.Headers;
			statusCode = response.StatusCode;
			taskStream = response.TaskStream;
			@object = response.Object;
		}
	}
}
