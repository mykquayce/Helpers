namespace Helpers.Elgato.Models.Generated;

public partial record LightObject
{
	public bool IsRgb => this.hue is not null && this.saturation is not null;
	public bool IsWhite => this.temperature is not null;
}
