using System;
using System.ComponentModel;

namespace Helpers.Cineworld.Models
{
	public partial class showType
	{
		private readonly static TimeZoneInfo _tz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

		public showType()
		{
			this.PropertyChanged += showType_PropertyChanged;
		}
		public DateTime DateTime { get; set; }


		private void showType_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is showType show)) return;

			switch (e.PropertyName)
			{
				case nameof(date):
				case nameof(time):
					if (string.IsNullOrWhiteSpace(show.date) || string.IsNullOrWhiteSpace(show.time)) break;

					var d = show.date.ParseDate();
					var t = show.time.ParseTime();

					var dateTime = d + t;

					if (_tz.IsDaylightSavingTime(dateTime))
					{
						dateTime += TimeSpan.FromHours(-1);
					}

					DateTime = dateTime;

					break;
			}
		}
	}
}
