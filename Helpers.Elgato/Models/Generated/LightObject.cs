namespace Helpers.Elgato.Models.Generated;

/// <summary>
/// represents the json sent/received from elgato hardware
/// </summary>
/// <param name="on">int [0..1]</param>
/// <param name="brightness">int [0..100]</param>
/// <param name="temperature">int [143..344]</param>
/// <param name="hue">double [0..360]</param>
/// <param name="saturation">double [0..100]</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd-party")]
public partial record LightObject(int on, int brightness, int? temperature, double? hue, double? saturation);
