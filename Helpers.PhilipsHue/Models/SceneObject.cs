using System;
using System.Collections.Generic;

namespace Helpers.PhilipsHue.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record SceneObject(
		string name, string type, string group, IList<string> lights, string owner, bool recycle,
		bool locked, SceneObject.AppdataObject appdata, string picture, DateTime lastupdated,
		int version, IDictionary<string, SceneObject.LightStateObject>? lightstates)
	{
		public record AppdataObject(int? version, string? data);
		public record LightStateObject(bool? on, byte? bri, IList<float>? xy);
	}
#pragma warning restore IDE1006 // Naming Styles
}
