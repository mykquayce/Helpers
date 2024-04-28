using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Helpers.Nanoleaf.Models;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct NumericValue<T>(T max, T min, T value)
	where T : INumber<T>;
