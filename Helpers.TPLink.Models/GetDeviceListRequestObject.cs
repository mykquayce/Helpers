namespace Helpers.TPLink.Models
{
	public record GetDeviceListRequestObject(string method = "getDeviceList") : IRequest;
}
