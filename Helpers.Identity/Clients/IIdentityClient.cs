namespace Helpers.Identity.Clients;

public interface IIdentityClient
{
	Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
