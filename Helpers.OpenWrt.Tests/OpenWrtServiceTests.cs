using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.OpenWrt.Tests
{
	public sealed class OpenWrtServiceTests : IClassFixture<Fixtures.OpenWrtServiceFixture>
	{
		private readonly Services.IOpenWrtService _sut;

		public OpenWrtServiceTests(Fixtures.OpenWrtServiceFixture fixture)
		{
			_sut = fixture.OpenWrtService;
		}

		[Theory]
		[InlineData("77.68.0.0/17")]
		[InlineData("77.68.11.211")]
		public async Task AddBlackhole(string s)
		{
			var subnetAddress = Networking.Models.SubnetAddress.Parse(s);

			// see if already exists
			var exists = await _sut.GetBlackholesAsync().AnyAsync(b => b == subnetAddress);

			// if so, remove it
			if (exists) await _sut.DeleteBlackholeAsync(subnetAddress);

			// Assert it was removed
			Assert.False(await _sut.GetBlackholesAsync().AnyAsync(b => b == subnetAddress));

			// add it
			await _sut.AddBlackholeAsync(subnetAddress);

			// Assert it was added
			Assert.True(await _sut.GetBlackholesAsync().AnyAsync(b => b == subnetAddress));

			// delete it
			await _sut.DeleteBlackholeAsync(subnetAddress);

			// Assert it was deleted
			Assert.False(await _sut.GetBlackholesAsync().AnyAsync(b => b == subnetAddress));

			// if it existed previously, put it back
			if (exists) await _sut.AddBlackholeAsync(subnetAddress);
		}
	}
}
