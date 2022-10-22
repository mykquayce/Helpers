using Dawn;
using System.Net.Http.Headers;

namespace Helpers.DockerHub.Concrete;

public class RegistryClient : Helpers.Web.WebClientBase, IRegistryClient
{
	private readonly IAuthorizationClient _authorizationClient;

	public RegistryClient(HttpClient httpClient, IAuthorizationClient authorizationClient)
		: base(httpClient)
	{
		_authorizationClient = Guard.Argument(authorizationClient).NotNull().Value;
	}

	public async IAsyncEnumerable<string> GetTagsAsync(string organization, string repository, CancellationToken cancellationToken = default)
	{
		Guard.Argument(organization).IsTagName();
		Guard.Argument(repository).IsTagName();
		var uri = new Uri($"/v2/{organization}/{repository}/tags/list", UriKind.Relative);
		var responseObject = await SendAsync<Models.TagsResponseObject>(organization, repository, uri, cancellationToken);

		var (_, tags) = responseObject;

		foreach (var tag in tags)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			yield return tag;
		}
	}

	public Task<Models.ManifestsResponseObject> GetManifestsAsync(string organization, string repository, string tag, CancellationToken cancellationToken = default)
	{
		Guard.Argument(organization).IsTagName();
		Guard.Argument(repository).IsTagName();
		Guard.Argument(tag).IsTagName();
		var uri = new Uri($"/v2/{organization}/{repository}/manifests/{tag}", UriKind.Relative);
		return SendAsync<Models.ManifestsResponseObject>(organization, repository, uri, cancellationToken);
	}

	private async Task<T> SendAsync<T>(string organization, string repository, Uri uri, CancellationToken cancellationToken = default)
		where T : class
	{
		Guard.Argument(organization).IsTagName();
		Guard.Argument(repository).IsTagName();
		var token = await _authorizationClient.GetTokenAsync(organization, repository, cancellationToken);

		var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
		{
			Headers =
			{
				Authorization = new AuthenticationHeaderValue("Bearer", token),
			},
		};

		var (_, _, responseObject) = await base.SendAsync<T>(requestMessage, cancellationToken);

		return responseObject;
	}
}
