using Helpers.Elgato.Models;

namespace Helpers.Elgato;

public interface IWhiteLightClient
{
	Task<Info> GetInfoAsync(CancellationToken cancellationToken = default);
	Task<WhiteLight> GetAsync(CancellationToken cancellationToken = default);
	Task<HttpResponseMessage> SetAsync(WhiteLight light, CancellationToken cancellationToken = default);
}
