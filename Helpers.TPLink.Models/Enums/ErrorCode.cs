namespace Helpers.TPLink.Models.Enums
{
	public enum ErrorCode : short
	{

		TokenExpired = -20_651,
		PasswordIncorrect = -20_601,
		DeviceIsOffline = -20_571,
		AccountLoginInOtherPlaces = -20_675,
		EmailFormatError = -20_200,
		MethodError = -20_103,
		Timeout = -20_002,
		JsonFormatError = -10_100,
		RequestMethodGetNotSupported = -10_000,
		None = 0,
	}
}
