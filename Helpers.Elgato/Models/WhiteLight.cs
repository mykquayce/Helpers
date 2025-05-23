﻿namespace Helpers.Elgato.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct WhiteLight(byte on, byte brightness, short temperature);
