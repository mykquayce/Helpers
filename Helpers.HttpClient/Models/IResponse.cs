using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.HttpClient.Models
{
	public interface IResponse
	{
		IReadOnlyDictionary<string, StringValues>? Headers { get; }
		HttpStatusCode? StatusCode { get; }
		Task<Stream>? TaskStream { get; }
	}

	public interface IResponse<T> : IResponse
		where T : class
	{
		T? Object { get; }
	}
}
