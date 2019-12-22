﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Helpers.Cineworld.Models
{
	public class Query
	{
		public string? Title { get; set; }
		public IList<short> CinemaIds { get; set; } = new List<short>();
		[JsonConverter(typeof(JsonStringFlagsEnumConverter<Enums.TimesOfDay>))]
		public Enums.TimesOfDay TimesOfDay { get; set; } = Enums.TimesOfDay.AllDay;
		[JsonConverter(typeof(JsonStringFlagsEnumConverter<Enums.DaysOfWeek>))]
		public Enums.DaysOfWeek DaysOfWeek { get; set; } = Enums.DaysOfWeek.AllWeek;
		public byte WeekCount { get; set; } = 1;
	}
}
