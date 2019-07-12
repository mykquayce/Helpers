using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Helpers.HttpClient
{
	public interface IHttpClient : IDisposable
	{
		Task<(HttpStatusCode, Stream, IDictionary<string, IEnumerable<string>>)> SendAsync(HttpMethod httpMethod, Uri uri, string? body = default, [CallerMemberName] string? methodName = default);
		Task<(HttpStatusCode, T, IDictionary<string, IEnumerable<string>>)> SendAsync<T>(HttpMethod httpMethod, Uri uri, string? body = default, [CallerMemberName] string? methodName = default);
	}
}