using System.Collections.Generic;
using System.Net;

namespace Helpers.HttpClient.Models
{
	public interface IResponseBase
	{
		IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; }
		HttpStatusCode? StatusCode { get; }
	}
}
