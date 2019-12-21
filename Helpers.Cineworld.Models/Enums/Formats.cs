using System;

namespace Helpers.Cineworld.Models.Enums
{
	[Flags]
	public enum Formats : short
	{
		None = 0,
		_2d = 1,
		_3d = 2,
		_4dx = 4,
		AutismFriendlyScreening = 8,
		Imax = 16,
		MoviesForJuniors = 32,
		ScreenX = 64,
		SecretUnlimitedScreening = 128,
		Subtitled = 256,
	}
}
