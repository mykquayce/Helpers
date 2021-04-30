using System;

namespace Helpers.TPLink.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record LoginRequestObject(string method, LoginRequestObject.ParamsObject @params)
		: IRequest
	{
		public LoginRequestObject(string cloudUserName, string cloudPassword)
			: this("login", new ParamsObject(cloudUserName, cloudPassword))
		{ }

		public record ParamsObject(string appType, string cloudUserName, string cloudPassword, Guid terminalUUID)
		{
			public ParamsObject(string cloudUserName, string cloudPassword)
				: this("Kasa_Android", cloudUserName, cloudPassword, Guid.NewGuid())
			{ }
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
