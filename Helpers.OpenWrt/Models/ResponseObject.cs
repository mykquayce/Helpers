namespace Helpers.OpenWrt.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record ResponseObject(int? id, string? result, ResponseObject.ErrorObject? error)
	{
		public ResponseObject() : this(default, default, default) { }

		public record ErrorObject(string? message, int? code)
		{
			public ErrorObject() : this(default, default) { }
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
