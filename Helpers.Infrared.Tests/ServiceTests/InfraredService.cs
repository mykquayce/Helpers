using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Infrared.Tests.ServiceTests
{
	public class InfraredService
	{
		private readonly Services.IInfraredService _sut;

		public InfraredService()
		{
			var devices = new Services.Concrete.InfraredService.Devices
			{
				["amp"] = "iTach059CAD",
			};

			var signals = new Services.Concrete.InfraredService.Signals
			{
				[Models.SignalTypes.ToggleMute]  = "sendir,1:1,3,40064,3,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r",
				[Models.SignalTypes.TogglePower] = "sendir,1:1,3,40192,3,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
				[Models.SignalTypes.VolumeUp]    = "sendir,1:1,3,40192,3,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
				[Models.SignalTypes.VolumeDown]  = "sendir,1:1,3,40192,3,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
			};

			var config = Clients.Concrete.GlobalCacheClient.Config.Defaults;
			var client = new Clients.Concrete.GlobalCacheClient(config);

			_sut = new Services.Concrete.InfraredService(client, Options.Create(devices), Options.Create(signals));
		}

		[Theory]
		[InlineData("amp")]
		public async Task ToggleMute(string deviceAlias)
		{
			await _sut.ToggleMuteAsync(deviceAlias);
			await Task.Delay(millisecondsDelay: 500);
			await _sut.ToggleMuteAsync(deviceAlias);
		}

		[Theory]
		[InlineData("amp")]
		public Task VolumeUp(string deviceAlias) => _sut.VolumeUpAsync(deviceAlias);

		[Theory]
		[InlineData("amp")]
		public Task VolumeDown(string deviceAlias) => _sut.VolumeDownAsync(deviceAlias);
	}
}
