using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class WhoIsClientTests
	{
		[Theory]
		[InlineData(32_934)]
		public async Task Test1(int asn)
		{
			var sut = new Helpers.Networking.Clients.Concrete.WhoIsClient();
			var tuples = sut.GetIpsAsync(asn);

			Assert.NotNull(tuples);

			await foreach (var (ip, mask) in tuples)
			{
				Assert.NotNull(ip);
				Assert.NotEmpty(ip.ToString());
				Assert.InRange(mask, 0, 64);
			}
		}
	}
}
