namespace Helpers.TPLink.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record LoginResponseObject(Enums.ErrorCode error_code, string msg, LoginResponseObject.LoginResponseResultObject result)
		: IResponse
	{
		public record LoginResponseResultObject(string accountId, string regTime, string countryCode, string email, string token);
	}
#pragma warning restore IDE1006 // Naming Styles
}
