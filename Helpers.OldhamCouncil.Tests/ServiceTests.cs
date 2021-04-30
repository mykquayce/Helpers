using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.OldhamCouncil.Tests
{
	public sealed class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
	{
		private readonly IService _sut;

		public ServiceTests(Fixtures.ServiceFixture fixture)
		{
			_sut = fixture.Service;
		}

		[Theory]
		[InlineData(422_000_014_726)]
		[InlineData(422_000_112_981)]
		public async Task GetBinCollections(long id)
		{
			var dictionary = await _sut.GetBinCollectionsAsync(id)
				.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value);

			Assert.NotNull(dictionary);
			Assert.NotEmpty(dictionary);
			Assert.All(dictionary.Keys, dt => Assert.NotEqual(default, dt));
			Assert.All(dictionary.Values, e => Assert.NotEqual(Models.BinTypes.None, e));
		}

		[Theory]
		[InlineData("OL1 1UT", default)]
		public async Task PostcodeAndHouseNumber(string postcode, string? houseNumber)
		{
			var dictionary = await _sut.GetBinCollectionsAsync(postcode, houseNumber)
				.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value);

			Assert.NotNull(dictionary);
			Assert.NotEmpty(dictionary);
			Assert.All(dictionary.Keys, dt => Assert.NotEqual(default, dt));
			Assert.All(dictionary.Values, e => Assert.NotEqual(Models.BinTypes.None, e));
		}
	}
}
