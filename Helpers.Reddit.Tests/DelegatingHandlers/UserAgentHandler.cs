namespace System.Net.Http;

public class UserAgentHandler(string userAgent) : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Headers.UserAgent.Count == 0)
		{
			request.Headers.UserAgent.TryParseAdd(userAgent);
		}

		return base.SendAsync(request, cancellationToken);
	}
}
