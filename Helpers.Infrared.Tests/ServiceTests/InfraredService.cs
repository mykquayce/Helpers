using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Infrared.Tests.ServiceTests
{
	public class InfraredService
	{
		private readonly Services.IInfraredService _sut;

		public InfraredService()
		{
			var devicesJson = @"{""amp"":""iTach059CAD""}";

			var signalsJson = @"{
				""ToggleMute"":""sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r"",
				""TogglePower"":""sendir,1:1,3,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r"",
				""VolumeUp"":""sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r"",
				""VolumeDown"":""sendir,1:1,3,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r""
			}";

			var devices = JsonSerializer.Deserialize<Services.Concrete.InfraredService.Devices>(devicesJson) ?? throw new Exception();
			var signals = JsonSerializer.Deserialize<Services.Concrete.InfraredService.Signals>(signalsJson) ?? throw new Exception();
			var client = new Clients.Concrete.GlobalCacheClient();

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
