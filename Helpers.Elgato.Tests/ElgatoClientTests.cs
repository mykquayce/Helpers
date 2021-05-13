using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Elgato.Tests
{
	public sealed class ElgatoClientTests : IClassFixture<Fixtures.ConfigFixture>, IClassFixture<Fixtures.ElgatoClientFixture>
	{
		private readonly IPAddress _ipAddress;
		private readonly IElgatoClient _sut;

		public ElgatoClientTests(
			Fixtures.ConfigFixture configFixture,
			Fixtures.ElgatoClientFixture clientFixture)
		{
			_ipAddress = configFixture.Addresses.IPAddress;
			_sut = clientFixture.Client;
		}

		[Fact]
		public async Task GetAccessoryInfo()
		{
			var info = await _sut.GetAccessoryInfoAsync(_ipAddress);

			Assert.NotNull(info);

			Assert.NotEmpty(info.productName);
			Assert.InRange(info.hardwareBoardType, 1, int.MaxValue);
			Assert.InRange(info.firmwareBuildNumber, 1, int.MaxValue);
			Assert.NotNull(info.firmwareVersion);
			Assert.NotEmpty(info.serialNumber);
			Assert.NotEmpty(info.displayName);
			Assert.NotEmpty(info.features);
			Assert.All(info.features, Assert.NotNull);
			Assert.All(info.features, Assert.NotEmpty);
		}

		[Fact]
		public async Task GetLight()
		{
			var light = await _sut.GetLightAsync(_ipAddress);

			Assert.NotNull(light);
			Assert.InRange(light.brightness, 0, 100);
			Assert.InRange(light.on, 0, 1);
			Assert.InRange(light.temperature, 140, 350);
		}

		[Theory]
		[InlineData(1, 23, 343)]
		[InlineData(0, 23, 343)]
		public async Task SetLight(byte on, byte brightness, short temperature)
		{
			var before = new Models.MessageObject.LightObject(on, brightness, temperature);

			await _sut.SetLightAsync(_ipAddress, before);

			var after = await _sut.GetLightAsync(_ipAddress);

			Assert.Equal(on, after.on);
			Assert.Equal(brightness, after.brightness);
			Assert.Equal(temperature, after.temperature);
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(1, 0)]
		public async Task ToggleLight_OffOn(byte before, byte after)
		{
			// Arrange
			var baseState = await _sut.GetLightAsync(_ipAddress);
			await _sut.SetLightAsync(_ipAddress, baseState with { on = before, });

			// Act
			await _sut.ToggleLightAsync(_ipAddress);

			// Assert
			var currentState = await _sut.GetLightAsync(_ipAddress);
			Assert.Equal(after, currentState.on);

			// Arrange
			await _sut.SetLightAsync(_ipAddress, baseState);
		}
	}
}
