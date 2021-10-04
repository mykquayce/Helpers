using System.Globalization;

namespace Helpers.Cineworld.Models.Generated.AllPerformances;

public partial class show
{
	private readonly static IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
	private const DateTimeStyles _dateTimeStyles = DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal;

	public DateTime DateTime
	{
		get
		{
			var date = DateTime.ParseExact(this.date, "ddd dd MMM", _formatProvider, _dateTimeStyles);
			var time = TimeSpan.ParseExact(this.time, "hh\\:mm", _formatProvider);
			return date + time;
		}
	}
}
