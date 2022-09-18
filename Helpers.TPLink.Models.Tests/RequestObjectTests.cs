using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Models.Tests
{
	public class RequestObjectTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(0)]
		public void RelayState(int state)
		{
			var request = new Generated.RequestObject(
				new Generated.RequestObject.SystemObject(
					new Generated.RequestObject.SystemObject.SetRelayStateObject(state)
					)
				);

			var json = request.Serialize();
			Assert.Equal($@"{{""system"":{{""set_relay_state"":{{""state"":{state:D}}}}}}}", json);
		}

		[Fact]
		public void SysInfo()
		{
			var request = new Generated.RequestObject(
				new Generated.RequestObject.SystemObject(
					new Generated.RequestObject.SystemObject.GetSysInfoObject()
					)
				);

			var json = request.Serialize();
			Assert.Equal(@"{""system"":{""get_sysinfo"":{}}}", json);
		}

		[Fact]
		public void RealtimeData()
		{
			var request = new Generated.RequestObject(
				new Generated.RequestObject.SystemObject(
					new Generated.RequestObject.SystemObject.GetSysInfoObject()
					),
				new Generated.RequestObject.EmeterObject(
					new Generated.RequestObject.EmeterObject.GetRealtimeObject()
					)
				);

			var json = request.Serialize();
			Assert.Equal(@"{""system"":{""get_sysinfo"":{}},""emeter"":{""get_realtime"":{}}}", json);
		}
	}
}
