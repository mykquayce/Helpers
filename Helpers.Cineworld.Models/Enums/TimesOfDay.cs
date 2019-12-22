using System;

namespace Helpers.Cineworld.Models.Enums
{
	[Flags]
	public enum TimesOfDay : byte
	{
		None = 0,

		Night = 1,
		Morning = 2,
		Afternoon = 4,
		Evening = 8,

		AM = Night | Morning,
		PM = Afternoon | Evening,

		AllDay = AM | PM,
	}
}
