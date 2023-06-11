using System.Diagnostics.CodeAnalysis;

namespace Helpers.Nanoleaf.Models;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct InfoResponse(
	string name,
	string serialNo,
	string manufacturer,
	string firmwareVersion,
	string hardwareVersion,
	string model,
	InfoResponse.Effects effects,
	InfoResponse.Panellayout panelLayout,
	InfoResponse.State state)
{
	public readonly record struct Effects(IReadOnlyList<string> effectsList, string select);
	public readonly record struct Panellayout(NumericValue<int> globalOrientation, Panellayout.Layout layout)
	{
		public readonly record struct Layout(int numPanels, int sideLength, IReadOnlyList<Layout.Positiondata> positionData)
		{
			public readonly record struct Positiondata(int panelId, int x, int y, int o, int shapeType);
		}
	}
	public readonly record struct State(
		NumericValue<int> brightness,
		string colorMode,
		NumericValue<int> ct,
		NumericValue<int> hue,
		BooleanValue on,
		NumericValue<int> sat);
}
