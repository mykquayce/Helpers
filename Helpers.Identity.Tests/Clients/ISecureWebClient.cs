namespace Helpers.Identity.Tests.Clients;

public interface ISecureWebClient
{
	Task<string> GetStringAsync(string relativeUri, CancellationToken? cancellationToken = null)
		=> GetStringAsync(new Uri(relativeUri, UriKind.RelativeOrAbsolute), cancellationToken);

	Task<string> GetStringAsync(Uri relativeUri, CancellationToken? cancellationToken = null);
}
