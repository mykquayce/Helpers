namespace Helpers.Elgato.Models.Lights;

public record WhiteLightModel(bool On, float Brightness, short Kelvins)
	:LightModel(On, Brightness);
