﻿using System.Drawing;
using Xunit;

namespace Helpers.Elgato.Tests;

public class ColorTests
{
	[Theory]
	[InlineData(KnownColor.ActiveBorder)]
	[InlineData(KnownColor.ActiveCaption)]
	[InlineData(KnownColor.ActiveCaptionText)]
	[InlineData(KnownColor.AliceBlue)]
	[InlineData(KnownColor.AntiqueWhite)]
	[InlineData(KnownColor.AppWorkspace)]
	[InlineData(KnownColor.Aqua)]
	[InlineData(KnownColor.Aquamarine)]
	[InlineData(KnownColor.Azure)]
	[InlineData(KnownColor.Beige)]
	[InlineData(KnownColor.Bisque)]
	[InlineData(KnownColor.Black)]
	[InlineData(KnownColor.BlanchedAlmond)]
	[InlineData(KnownColor.Blue)]
	[InlineData(KnownColor.BlueViolet)]
	[InlineData(KnownColor.Brown)]
	[InlineData(KnownColor.BurlyWood)]
	[InlineData(KnownColor.ButtonFace)]
	[InlineData(KnownColor.ButtonHighlight)]
	[InlineData(KnownColor.ButtonShadow)]
	[InlineData(KnownColor.CadetBlue)]
	[InlineData(KnownColor.Chartreuse)]
	[InlineData(KnownColor.Chocolate)]
	[InlineData(KnownColor.Control)]
	[InlineData(KnownColor.ControlDark)]
	[InlineData(KnownColor.ControlDarkDark)]
	[InlineData(KnownColor.ControlLight)]
	[InlineData(KnownColor.ControlLightLight)]
	[InlineData(KnownColor.ControlText)]
	[InlineData(KnownColor.Coral)]
	[InlineData(KnownColor.CornflowerBlue)]
	[InlineData(KnownColor.Cornsilk)]
	[InlineData(KnownColor.Crimson)]
	[InlineData(KnownColor.Cyan)]
	[InlineData(KnownColor.DarkBlue)]
	[InlineData(KnownColor.DarkCyan)]
	[InlineData(KnownColor.DarkGoldenrod)]
	[InlineData(KnownColor.DarkGray)]
	[InlineData(KnownColor.DarkGreen)]
	[InlineData(KnownColor.DarkKhaki)]
	[InlineData(KnownColor.DarkMagenta)]
	[InlineData(KnownColor.DarkOliveGreen)]
	[InlineData(KnownColor.DarkOrange)]
	[InlineData(KnownColor.DarkOrchid)]
	[InlineData(KnownColor.DarkRed)]
	[InlineData(KnownColor.DarkSalmon)]
	[InlineData(KnownColor.DarkSeaGreen)]
	[InlineData(KnownColor.DarkSlateBlue)]
	[InlineData(KnownColor.DarkSlateGray)]
	[InlineData(KnownColor.DarkTurquoise)]
	[InlineData(KnownColor.DarkViolet)]
	[InlineData(KnownColor.DeepPink)]
	[InlineData(KnownColor.DeepSkyBlue)]
	[InlineData(KnownColor.Desktop)]
	[InlineData(KnownColor.DimGray)]
	[InlineData(KnownColor.DodgerBlue)]
	[InlineData(KnownColor.Firebrick)]
	[InlineData(KnownColor.FloralWhite)]
	[InlineData(KnownColor.ForestGreen)]
	[InlineData(KnownColor.Fuchsia)]
	[InlineData(KnownColor.Gainsboro)]
	[InlineData(KnownColor.GhostWhite)]
	[InlineData(KnownColor.Gold)]
	[InlineData(KnownColor.Goldenrod)]
	[InlineData(KnownColor.GradientActiveCaption)]
	[InlineData(KnownColor.GradientInactiveCaption)]
	[InlineData(KnownColor.Gray)]
	[InlineData(KnownColor.GrayText)]
	[InlineData(KnownColor.Green)]
	[InlineData(KnownColor.GreenYellow)]
	[InlineData(KnownColor.Highlight)]
	[InlineData(KnownColor.HighlightText)]
	[InlineData(KnownColor.Honeydew)]
	[InlineData(KnownColor.HotPink)]
	[InlineData(KnownColor.HotTrack)]
	[InlineData(KnownColor.InactiveBorder)]
	[InlineData(KnownColor.InactiveCaption)]
	[InlineData(KnownColor.InactiveCaptionText)]
	[InlineData(KnownColor.IndianRed)]
	[InlineData(KnownColor.Indigo)]
	[InlineData(KnownColor.Info)]
	[InlineData(KnownColor.InfoText)]
	[InlineData(KnownColor.Ivory)]
	[InlineData(KnownColor.Khaki)]
	[InlineData(KnownColor.Lavender)]
	[InlineData(KnownColor.LavenderBlush)]
	[InlineData(KnownColor.LawnGreen)]
	[InlineData(KnownColor.LemonChiffon)]
	[InlineData(KnownColor.LightBlue)]
	[InlineData(KnownColor.LightCoral)]
	[InlineData(KnownColor.LightCyan)]
	[InlineData(KnownColor.LightGoldenrodYellow)]
	[InlineData(KnownColor.LightGray)]
	[InlineData(KnownColor.LightGreen)]
	[InlineData(KnownColor.LightPink)]
	[InlineData(KnownColor.LightSalmon)]
	[InlineData(KnownColor.LightSeaGreen)]
	[InlineData(KnownColor.LightSkyBlue)]
	[InlineData(KnownColor.LightSlateGray)]
	[InlineData(KnownColor.LightSteelBlue)]
	[InlineData(KnownColor.LightYellow)]
	[InlineData(KnownColor.Lime)]
	[InlineData(KnownColor.LimeGreen)]
	[InlineData(KnownColor.Linen)]
	[InlineData(KnownColor.Magenta)]
	[InlineData(KnownColor.Maroon)]
	[InlineData(KnownColor.MediumAquamarine)]
	[InlineData(KnownColor.MediumBlue)]
	[InlineData(KnownColor.MediumOrchid)]
	[InlineData(KnownColor.MediumPurple)]
	[InlineData(KnownColor.MediumSeaGreen)]
	[InlineData(KnownColor.MediumSlateBlue)]
	[InlineData(KnownColor.MediumSpringGreen)]
	[InlineData(KnownColor.MediumTurquoise)]
	[InlineData(KnownColor.MediumVioletRed)]
	[InlineData(KnownColor.Menu)]
	[InlineData(KnownColor.MenuBar)]
	[InlineData(KnownColor.MenuHighlight)]
	[InlineData(KnownColor.MenuText)]
	[InlineData(KnownColor.MidnightBlue)]
	[InlineData(KnownColor.MintCream)]
	[InlineData(KnownColor.MistyRose)]
	[InlineData(KnownColor.Moccasin)]
	[InlineData(KnownColor.NavajoWhite)]
	[InlineData(KnownColor.Navy)]
	[InlineData(KnownColor.OldLace)]
	[InlineData(KnownColor.Olive)]
	[InlineData(KnownColor.OliveDrab)]
	[InlineData(KnownColor.Orange)]
	[InlineData(KnownColor.OrangeRed)]
	[InlineData(KnownColor.Orchid)]
	[InlineData(KnownColor.PaleGoldenrod)]
	[InlineData(KnownColor.PaleGreen)]
	[InlineData(KnownColor.PaleTurquoise)]
	[InlineData(KnownColor.PaleVioletRed)]
	[InlineData(KnownColor.PapayaWhip)]
	[InlineData(KnownColor.PeachPuff)]
	[InlineData(KnownColor.Peru)]
	[InlineData(KnownColor.Pink)]
	[InlineData(KnownColor.Plum)]
	[InlineData(KnownColor.PowderBlue)]
	[InlineData(KnownColor.Purple)]
	[InlineData(KnownColor.RebeccaPurple)]
	[InlineData(KnownColor.Red)]
	[InlineData(KnownColor.RosyBrown)]
	[InlineData(KnownColor.RoyalBlue)]
	[InlineData(KnownColor.SaddleBrown)]
	[InlineData(KnownColor.Salmon)]
	[InlineData(KnownColor.SandyBrown)]
	[InlineData(KnownColor.ScrollBar)]
	[InlineData(KnownColor.SeaGreen)]
	[InlineData(KnownColor.SeaShell)]
	[InlineData(KnownColor.Sienna)]
	[InlineData(KnownColor.Silver)]
	[InlineData(KnownColor.SkyBlue)]
	[InlineData(KnownColor.SlateBlue)]
	[InlineData(KnownColor.SlateGray)]
	[InlineData(KnownColor.Snow)]
	[InlineData(KnownColor.SpringGreen)]
	[InlineData(KnownColor.SteelBlue)]
	[InlineData(KnownColor.Tan)]
	[InlineData(KnownColor.Teal)]
	[InlineData(KnownColor.Thistle)]
	[InlineData(KnownColor.Tomato)]
	[InlineData(KnownColor.Transparent)]
	[InlineData(KnownColor.Turquoise)]
	[InlineData(KnownColor.Violet)]
	[InlineData(KnownColor.Wheat)]
	[InlineData(KnownColor.White)]
	[InlineData(KnownColor.WhiteSmoke)]
	[InlineData(KnownColor.Window)]
	[InlineData(KnownColor.WindowFrame)]
	[InlineData(KnownColor.WindowText)]
	[InlineData(KnownColor.Yellow)]
	[InlineData(KnownColor.YellowGreen)]
	public void Colors(KnownColor knownColor)
	{
		var before = Color.FromKnownColor(knownColor);
		var hsbColor = before.GetHsbColor();
		var after = hsbColor.GetColor();

		Assert.Equal(before.R, after.R);
		Assert.Equal(before.G, after.G);
		Assert.Equal(before.B, after.B);
	}
}
