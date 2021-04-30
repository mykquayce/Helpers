using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.OldhamCouncil.Tests
{
	public sealed class ClientTests : IClassFixture<Fixtures.ClientFixture>
	{
		private readonly IClient _sut;

		public ClientTests(Fixtures.ClientFixture fixture)
		{
			_sut = fixture.Client;
		}

		[Theory]
		[InlineData("ol11aa", 422_000_069_073, "21  OLDHAM DELIVERY OFFICE HAMILTON STREET OLDHAM OL1 1AA")]
		[InlineData("ol1 1ut", 422_000_112_981, "CIVIC CENTRE WEST STREET OLDHAM OL1 1UT")]
		public async Task GetAddresses(string postcode, long id, string address)
		{
			var dictionary = await _sut.GetAddressesAsync(postcode)
				.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value);

			Assert.NotNull(dictionary);
			Assert.NotEmpty(dictionary);
			Assert.NotNull(dictionary);
			Assert.NotEmpty(dictionary);
			Assert.Contains(id, dictionary.Keys);
			Assert.Equal(address, dictionary[id]);
		}
	}
}
