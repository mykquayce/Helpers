using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Infrared.Services.Concrete
{
	public class InfraredService : IInfraredService
	{
		public record Devices() : Models.DictionaryRecord<string, string> { }
		public record Signals() : Models.DictionaryRecord<Models.SignalTypes, string> { }

		private readonly Clients.IClient _client;
		private readonly Devices _devices;
		private readonly Signals _signals;

		public InfraredService(
			Clients.IClient client,
			IOptions<Devices> devices,
			IOptions<Signals> signals)
		{
			_client = Guard.Argument(() => client).NotNull().Value;
			_devices = Guard.Argument(() => devices).NotNull().Wrap(o => o.Value).NotNull().NotEmpty().Value;
			_signals = Guard.Argument(() => signals).NotNull().Wrap(o => o.Value).NotNull().NotEmpty().Value;
		}

		public Task SendSignal(string deviceAlias, Models.SignalTypes signalType)
		{
			Guard.Argument(() => deviceAlias).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => signalType).NotDefault().Defined();

			if (!_devices.TryGetValue(deviceAlias, out var host))
			{
				throw new Exceptions.UnknownDeviceException(deviceAlias);
			}

			if (!_signals.TryGetValue(signalType, out var signal))
			{
				throw new Exceptions.UnknownSignalException(signalType);
			}

			return _client.SendAsync(host, signal);
		}

		public Task ToggleMuteAsync(string deviceAlias) => SendSignal(deviceAlias, Models.SignalTypes.ToggleMute);

		public Task TogglePowerAsync(string deviceAlias) => SendSignal(deviceAlias, Models.SignalTypes.TogglePower);

		public Task VolumeDownAsync(string deviceAlias) => SendSignal(deviceAlias, Models.SignalTypes.VolumeDown);

		public Task VolumeUpAsync(string deviceAlias) => SendSignal(deviceAlias, Models.SignalTypes.VolumeUp);
	}
}
