using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace System.Net.Http;

public class IdentityServerHandler(HttpClient httpClient, IOptions<IdentityServerHandler.IConfig> config)
	: DelegatingHandler
{
	private readonly IConfig _config = config?.Value ?? throw new ArgumentNullException(nameof(config));

	public interface IConfig
	{
		Uri Authority { get; set; }
		string ClientId { get; set; }
		string ClientSecret { get; set; }
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Headers.Authorization != null)
		{
			return await base.SendAsync(request, cancellationToken);
		}

		var token = await GetAccessTokenAsync(cancellationToken);

		request.Headers.Authorization = new Headers.AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken);
	}

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		var now = DateTimeOffset.UtcNow;
		var values = new Dictionary<string, string>
		{
			["client_id"] = _config.ClientId,
			["client_secret"] = _config.ClientSecret,
			["grant_type"] = "client_credentials",
		};

		var content = new FormUrlEncodedContent(values);

		var response = await httpClient.PostAsync("connect/token", content, cancellationToken);

		var (access_token, _, _, _) = await response.Content.ReadFromJsonAsync<TokenResponseObject>(cancellationToken);

		return access_token;
	}

#pragma warning disable IDE1006 // Naming Styles
		private readonly record struct TokenResponseObject(string access_token, int expires_in, string token_type, string scope);
#pragma warning restore IDE1006 // Naming Styles
}
