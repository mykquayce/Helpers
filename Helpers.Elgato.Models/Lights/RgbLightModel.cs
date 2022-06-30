using System.Drawing;

namespace Helpers.Elgato.Models.Lights;

public record RgbLightModel(bool On, float Brightness, Color Color)
	:LightModel(On, Brightness);
