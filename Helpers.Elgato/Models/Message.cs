namespace Helpers.Elgato.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct Message<T>(int numberOfLights, IReadOnlyCollection<T> lights);
