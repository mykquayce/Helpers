namespace IdentityModel.Client;

public class ProtocolResponseException : Exception
{
	public ProtocolResponseException(in ProtocolResponse response)
		: base($"error {response.ErrorType} ({response.Error})")
	{
		Data.Add(nameof(response.ErrorType), response.ErrorType);
		Data.Add(nameof(response.Error), response.Error);
		Data.Add(nameof(response.HttpErrorReason), response.HttpErrorReason);
		Data.Add(nameof(response.Raw), response.Raw);
	}
}
