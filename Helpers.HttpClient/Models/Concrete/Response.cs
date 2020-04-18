using System.IO;
using System.Threading.Tasks;

namespace Helpers.HttpClient.Models.Concrete
{
	public class Response : ResponseBase, IResponse
	{
		public Task<Stream>? TaskStream { get; set; }
	}

	public class Response<T> : ResponseBase, IResponse<T>
		where T : class
	{
		public T? Object { get; set; }
	}
}
