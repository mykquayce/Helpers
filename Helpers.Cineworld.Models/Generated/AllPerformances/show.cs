using System.Globalization;

namespace Helpers.Cineworld.Models.Generated.AllPerformances;

public partial class show
{
	private readonly static IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
	private const DateTimeStyles _dateTimeStyles = DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal;

	internal static Func<DateTime> GetTodayFunc { get; set; } = () => DateTime.UtcNow.Date;

	public DateTime DateTime
	{
		get
		{
			var date = DateTime.ParseExact(this.date[4..], "dd MMM", _formatProvider, _dateTimeStyles);
			if (date < GetTodayFunc()) date = date.AddYears(1);
			var time = TimeSpan.ParseExact(this.time, "hh\\:mm", _formatProvider);
			return date + time;
		}
	}
}
