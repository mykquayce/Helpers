using Microsoft.Extensions.Primitives;
using System.Net;

namespace Helpers.Web.Models.Concrete
{
	public record ResponseBase(IReadOnlyDictionary<string, StringValues> Headers, HttpStatusCode StatusCode)
		: IResponseBase;

	public record Response(IReadOnlyDictionary<string, StringValues> Headers, HttpStatusCode StatusCode, Task<Stream> TaskStream)
		: ResponseBase(Headers, StatusCode), IResponse;

	public record Response<T>(IReadOnlyDictionary<string, StringValues> Headers, HttpStatusCode StatusCode, T Object)
		: ResponseBase(Headers, StatusCode), IResponse<T>
		where T : class;
}
