using System.Threading.Tasks;

namespace Helpers.Infrared.Services
{
	public interface IInfraredService
	{
		Task SendSignal(string deviceAlias, Models.SignalTypes signalType);
		Task ToggleMuteAsync(string deviceAlias);
		Task TogglePowerAsync(string deviceAlias);
		Task VolumeDownAsync(string deviceAlias);
		Task VolumeUpAsync(string deviceAlias);
	}
}
