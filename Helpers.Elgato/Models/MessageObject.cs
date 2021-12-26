namespace Helpers.Elgato.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd-party")]
public record MessageObject(int numberOfLights, IReadOnlyCollection<MessageObject.LightObject> lights)
{
	public record LightObject(byte on, byte brightness, short temperature);
}
