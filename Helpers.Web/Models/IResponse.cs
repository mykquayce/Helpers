using Microsoft.Extensions.Primitives;
using System.Net;

namespace Helpers.Web.Models
{
	public interface IResponseBase
	{
		IReadOnlyDictionary<string, StringValues> Headers { get; }
		HttpStatusCode StatusCode { get; }
	}

	public interface IResponse : IResponseBase
	{
		Task<Stream> TaskStream { get; }
	}

	public interface IResponse<T> : IResponseBase
		where T : class
	{
		T Object { get; }
	}
}
