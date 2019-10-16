using Dawn;
using System;

namespace Helpers.Cineworld.Models
{
	public partial class showType
	{
		public DateTime DateTime
		{
			get
			{
				Guard.Argument(() => date)
					.NotNull()
					.NotEmpty()
					.NotWhiteSpace()
					.Matches(@"^(?:Sun|Mon|Tue|Wed|Thu|Fri|Sat) (?:[012]\d|3[01]) (?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)$");

				Guard.Argument(() => time)
					.NotNull()
					.NotEmpty()
					.NotWhiteSpace()
					.Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d$");

				var dayOfMonth = int.Parse(date[4..6]);

				var monthOfYear = date[7..] switch
				{
					"Jan" => 1,
					"Feb" => 2,
					"Mar" => 3,
					"Apr" => 4,
					"May" => 5,
					"Jun" => 6,
					"Jul" => 7,
					"Aug" => 8,
					"Sep" => 9,
					"Oct" => 10,
					"Nov" => 11,
					"Dec" => 12,
					_ => throw new ArgumentOutOfRangeException(nameof(date), date, "Unexpected month of year") { Data = { [nameof(date)] = date, }, },
				};

				var now = DateTime.UtcNow;
				var year = monthOfYear < now.Month
					? now.Year + 1
					: now.Year;

				var dateTime = DateTime.Parse(
					$"{year:D4}-{monthOfYear:D2}-{dayOfMonth:D2}T{time}:00",
					styles: System.Globalization.DateTimeStyles.AssumeLocal);

				return dateTime;
			}
		}
	}
}
