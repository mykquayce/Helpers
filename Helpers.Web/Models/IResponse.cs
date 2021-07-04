using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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
