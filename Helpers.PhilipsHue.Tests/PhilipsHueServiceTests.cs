using System.Threading.Tasks;
using Xunit;

namespace Helpers.PhilipsHue.Tests
{
	public class PhilipsHueServiceTests : IClassFixture<Fixtures.ClientFixture>
	{
		private readonly Services.IPhilipsHueService _sut;

		public PhilipsHueServiceTests(Fixtures.ClientFixture clientFixture)
		{
			_sut = new Services.Concrete.PhilipsHueService(clientFixture.Client);
		}

		[Theory]
		[InlineData("wall left")]
		[InlineData("wall right")]
		[InlineData("wall left", "wall right", "strip")]
		[InlineData("wall left", "strip")]
		public Task ToggleLight(params string[] names) => _sut.ToggleLightsAsync(names);

		[Fact]
		public Task SetLightsOnBrightRed()
		{
			var state = Models.LightObject.StateObject.On
				+ Models.LightObject.StateObject.Bright
				+ Models.LightObject.StateObject.Red;

			return _sut.SetLightsStateAsync(state, "wall left", "wall right", "strip");
		}

		[Fact]
		public Task SetLightsOnBrightWarm()
		{
			var state = Models.LightObject.StateObject.On
				+ Models.LightObject.StateObject.Bright
				+ Models.LightObject.StateObject.Warm;

			return _sut.SetLightsStateAsync(state, "wall left", "wall right", "strip");
		}

		[Theory]
		[InlineData("bedroom")]
		public Task ToggleLightsInGroups(params string[] groupNames) => _sut.ToggleLightsInGroupsAsync(groupNames);

		[Theory]
		[InlineData("bedroom", "main", true)]
		[InlineData("bedroom", "main", false)]
		public Task SetLightState(string groupName, string lightName, bool on)
		{
			var state = on
				? Models.LightObject.StateObject.On
				: Models.LightObject.StateObject.Off;

			return _sut.SetLightStateAsync(groupName, lightName, state);
		}

		[Fact]
		public Task SetBedroomMainLightOnColdestBrightest()
		{
			var state = Models.LightObject.StateObject.On
				+ Models.LightObject.StateObject.Coldest
				+ Models.LightObject.StateObject.Brightest;

			return _sut.SetLightStateAsync("bedroom", "main", state);
		}
	}
}
