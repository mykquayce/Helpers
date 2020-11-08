using System.Collections.Generic;

namespace Helpers.Elgato.Models
{
	public record MessageObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public int? numberOfLights { get; init; }
		public IList<LightObject>? lights { get; init; }

		public record LightObject
		{
			public byte? on { get; init; }
			public byte? brightness { get; init; }
			public short? temperature { get; init; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
