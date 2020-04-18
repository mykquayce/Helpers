using System.IO;
using System.Threading.Tasks;

namespace Helpers.HttpClient.Models
{
	public interface IResponse : IResponseBase
	{
		Task<Stream>? TaskStream { get; }
	}

	public interface IResponse<T> : IResponseBase
		where T : class
	{
		T? Object { get; }
	}
}
