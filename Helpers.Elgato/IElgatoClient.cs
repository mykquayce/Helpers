namespace Helpers.Elgato;

public interface IElgatoClient
{
	Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.MessageObject.LightObject> GetLightAsync(CancellationToken? cancellationToken = default);
	Task SetLightAsync(Models.MessageObject.LightObject light, CancellationToken? cancellationToken = default);
}
