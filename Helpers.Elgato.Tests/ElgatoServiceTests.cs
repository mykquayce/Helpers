using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Elgato.Tests
{
	public class ElgatoServiceTests : IClassFixture<Fixtures.SSHServiceFixture>
	{
		private readonly IElgatoService _sut;

		public ElgatoServiceTests(Fixtures.SSHServiceFixture ssHServiceFixture)
		{
			var sshService = ssHServiceFixture.SSHService;
			_sut = new Concrete.ElgatoService(sshService);
		}

		[Theory]
		[InlineData("3C6A9D14D765")]
		public async Task GetAccessoryInfo(string physicalAddressString)
		{
			var physicalAdddress = PhysicalAddress.Parse(physicalAddressString);
			var info = await _sut.GetAccessoryInfoAsync(physicalAdddress);

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

		[Theory]
		[InlineData("3C6A9D14D765")]
		public async Task GetLight(string physicalAddressString)
		{
			var physicalAdddress = PhysicalAddress.Parse(physicalAddressString);
			var light = await _sut.GetLightAsync(physicalAdddress);

			Assert.NotNull(light);
			Assert.NotNull(light.brightness);
			Assert.InRange(light.brightness!.Value, 0, 100);
			Assert.NotNull(light.on);
			Assert.Contains(light.on!.Value, new byte[2] { 0, 1, });
			Assert.NotNull(light.temperature);
			Assert.InRange(light.temperature!.Value, 140, 350);
		}

		[Theory]
		[InlineData("3C6A9D14D765", 1, 23, 343)]
		[InlineData("3C6A9D14D765", 0, 23, 343)]
		public Task SetLight(string physicalAddressString, int? on, int? brightness, int? temperature)
		{
			var physicalAdddress = PhysicalAddress.Parse(physicalAddressString);
			var light = new Models.MessageObject.LightObject((byte?)on, (byte?)brightness, (byte?)temperature);

			return _sut.SetLightAsync(physicalAdddress, light);
		}

		[Theory]
		[InlineData("3C6A9D14D765")]
		public Task ToggleLight(string physicalAddressString)
		{
			var physicalAdddress = PhysicalAddress.Parse(physicalAddressString);
			return _sut.ToggleLightPowerStateAsync(physicalAdddress);
		}
	}
}
