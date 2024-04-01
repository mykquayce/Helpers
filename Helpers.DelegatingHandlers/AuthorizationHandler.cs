namespace System.Net.Http;

public class AuthorizationHandler(AuthorizationHandler.ITokenGetter tokenGetter) : DelegatingHandler
{
	public interface ITokenGetter
	{
		ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default);
	}

	protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Headers.Contains("Authorization"))
		{
			return await base.SendAsync(request, cancellationToken);
		}

		var token = await tokenGetter.GetTokenAsync(cancellationToken);

		request.Headers.Authorization = new Headers.AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken);
	}
}
