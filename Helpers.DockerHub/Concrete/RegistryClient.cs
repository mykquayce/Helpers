using Dawn;
using System.Net.Http.Headers;

namespace Helpers.DockerHub.Concrete;

public class RegistryClient : Helpers.Web.WebClientBase, IRegistryClient
{
	private readonly IAuthorizationClient _authorizationClient;
	private readonly string _organization, _repository;

	public RegistryClient(HttpClient httpClient, IAuthorizationClient authorizationClient)
		: base(httpClient)
	{
		_authorizationClient = Guard.Argument(authorizationClient).NotNull().Value;
		_organization = Guard.Argument(_authorizationClient.Organization).NotNull().NotEmpty().NotWhiteSpace().Value;
		_repository = Guard.Argument(_authorizationClient.Repository).NotNull().NotEmpty().NotWhiteSpace().Value;
	}

	public async IAsyncEnumerable<string> GetTagsAsync(CancellationToken? cancellationToken = default)
	{
		var uri = new Uri($"/v2/{_organization}/{_repository}/tags/list", UriKind.Relative);
		var responseObject = await SendAsync<Models.TagsResponseObject>(uri, cancellationToken);

		var (_, tags) = responseObject;

		using var enumerator = tags.GetEnumerator();

		while (enumerator.MoveNext()
			&& cancellationToken?.IsCancellationRequested != true)
		{
			yield return enumerator.Current;
		}
	}

	public Task<Models.ManifestsResponseObject> GetManifestsAsync(string tag, CancellationToken? cancellationToken = default)
	{
		var uri = new Uri($"/v2/{_organization}/{_repository}/manifests/{tag}", UriKind.Relative);
		return SendAsync<Models.ManifestsResponseObject>(uri, cancellationToken);
	}

	private async Task<T> SendAsync<T>(Uri uri, CancellationToken? cancellationToken = default)
		where T : class
	{
		var token = await _authorizationClient.GetTokenAsync();

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
