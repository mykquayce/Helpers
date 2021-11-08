using Microsoft.Extensions.Primitives;
using System.Net;

namespace Helpers.Web.Models;

public interface IResponseBase
{
	IReadOnlyDictionary<string, StringValues> Headers { get; }
	HttpStatusCode StatusCode { get; }
	void Deconstruct(out IReadOnlyDictionary<string, StringValues> Headers, out HttpStatusCode StatusCode);
}

public interface IResponse : IResponseBase
{
	Task<Stream> TaskStream { get; }
	void Deconstruct(out IReadOnlyDictionary<string, StringValues> Headers, out HttpStatusCode StatusCode, out Task<Stream> TaskStream);
}

public interface IResponse<T> : IResponseBase
	where T : class
{
	T Object { get; }
	void Deconstruct(out IReadOnlyDictionary<string, StringValues> Headers, out HttpStatusCode StatusCode, out T @Object);
}
