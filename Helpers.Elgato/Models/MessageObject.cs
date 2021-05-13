using System.Collections.Generic;

namespace Helpers.Elgato.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record MessageObject(int numberOfLights, IList<MessageObject.LightObject> lights)
	{
		public record LightObject(byte on, byte brightness, short temperature);
	}
#pragma warning restore IDE1006 // Naming Styles
}
