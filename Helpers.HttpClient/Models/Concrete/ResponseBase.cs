using System.Collections.Generic;
using System.Net;

namespace Helpers.HttpClient.Models.Concrete
{
	public class ResponseBase : IResponseBase
	{
		public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; set; }
		public HttpStatusCode? StatusCode { get; set; }

	}
}
