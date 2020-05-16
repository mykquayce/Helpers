using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.HttpClient.Models.Concrete
{
	public class Response : IResponse
	{
		public IReadOnlyDictionary<string, StringValues>? Headers { get; set; }
		public HttpStatusCode? StatusCode { get; set; }
		public Task<Stream>? TaskStream { get; set; }
	}

	public class Response<T> : Response, IResponse<T>
		where T : class
	{
		public T? Object { get; set; }
	}
}
