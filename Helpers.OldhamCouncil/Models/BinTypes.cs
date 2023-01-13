using System.ComponentModel;

namespace Helpers.OldhamCouncil.Models;

[Flags]
public enum BinTypes : short
{
	None = 0,
	[Description("Blue")]
	Blue = 1,
	[Description("Brown")]
	Brown = 2,
	[Description("Green")]
	Green = 4,
	[Description("Grey")]
	Grey = 8,
	[Description("Violet")]
	Violet = 16,
	[Description("120")]
	_120 = 32,
	[Description("240")]
	_240 = 64,
	[Description("340")]
	_340 = 128,
	[Description("360")]
	_360 = 256,
	[Description("770")]
	_770 = 512,
	[Description("1100")]
	_1100 = 1_024,
}
