namespace Helpers.Identity.Tests.Clients;

public interface ISecureWebClient
{
	Task<string?> GetStringAsync(string relativeUri, CancellationToken cancellationToken = default)
		=> GetStringAsync(new Uri(relativeUri, UriKind.RelativeOrAbsolute), cancellationToken);

	Task<string?> GetStringAsync(Uri relativeUri, CancellationToken cancellationToken = default);
}
