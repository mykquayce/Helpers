using System.Threading.Tasks;
using Xunit;

namespace Helpers.Elgato.Tests
{
	[Collection("httpclient collection")]
	public class ElgatoServiceTests
	{
		private readonly Services.IElgatoService _sut;

		public ElgatoServiceTests(Fixtures.HttpClientFixture fixture)
		{
			var client = new Clients.Concrete.ElgatoClient(fixture.HttpClient);
			_sut = new Services.Concrete.ElgatoService(client);
		}

		[Fact]
		public async Task GetAccessoryInfo()
		{
			var info = await _sut.GetAccessoryInfoAsync();

			Assert.NotNull(info);

			Assert.NotNull(info.productName);
			Assert.NotNull(info.hardwareBoardType);
			Assert.NotNull(info.firmwareBuildNumber);
			Assert.NotNull(info.firmwareVersion);
			Assert.NotNull(info.serialNumber);
			Assert.NotNull(info.displayName);
			Assert.NotNull(info.features);

			Assert.NotEmpty(info.productName);
			Assert.InRange(info.hardwareBoardType!.Value, 1, int.MaxValue);
			Assert.InRange(info.firmwareBuildNumber!.Value, 1, int.MaxValue);
			Assert.NotEmpty(info.firmwareVersion);
			Assert.NotEmpty(info.serialNumber);
			Assert.NotEmpty(info.displayName);
			Assert.NotEmpty(info.features);
			Assert.All(info.features, Assert.NotNull);
			Assert.All(info.features, Assert.NotEmpty);
		}

		[Fact]
		public async Task GetLight()
		{
			var light = await _sut.GetLightAsync();

			Assert.NotNull(light);
			Assert.NotNull(light.brightness);
			Assert.InRange(light.brightness!.Value, 0, 100);
			Assert.NotNull(light.on);
			Assert.Contains(light.on!.Value, new byte[2] { 0, 1, });
			Assert.NotNull(light.temperature);
			Assert.InRange(light.temperature!.Value, 140, 350);
		}

		[Theory]
		[InlineData(1, 23, 343)]
		[InlineData(0, 23, 343)]
		public Task SetLight(int? on, int? brightness, int? temperature)
		{
			var light = new Models.MessageObject.LightObject((byte?)on, (byte?)brightness, (byte?)temperature);

			return _sut.SetLightAsync(light);
		}

		[Fact]
		public Task ToggleLight() => _sut.ToggleLightPowerStateAsync();
	}
}
