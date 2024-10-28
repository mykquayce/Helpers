using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Helpers.DockerHub.Concrete;

public class RegistryClient : Helpers.Web.WebClientBase, IRegistryClient
{
	private readonly IAuthorizationClient _authorizationClient;

	public RegistryClient(HttpClient httpClient, IAuthorizationClient authorizationClient)
		: base(httpClient)
	{
		ArgumentNullException.ThrowIfNull(authorizationClient);
		_authorizationClient = authorizationClient;
	}

	public async IAsyncEnumerable<string> GetTagsAsync(string organization, string repository, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(organization);
		ArgumentException.ThrowIfNullOrWhiteSpace(repository);
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
		ArgumentException.ThrowIfNullOrWhiteSpace(organization);
		ArgumentException.ThrowIfNullOrWhiteSpace(repository);
		ArgumentException.ThrowIfNullOrWhiteSpace(tag);
		var uri = new Uri($"/v2/{organization}/{repository}/manifests/{tag}", UriKind.Relative);
		return SendAsync<Models.ManifestsResponseObject>(organization, repository, uri, cancellationToken);
	}

	private async Task<T> SendAsync<T>(string organization, string repository, Uri uri, CancellationToken cancellationToken = default)
		where T : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(organization);
		ArgumentException.ThrowIfNullOrWhiteSpace(repository);
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
