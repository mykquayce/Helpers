using System.Diagnostics.CodeAnalysis;

namespace Helpers.Nanoleaf.Models;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct TokenResponse(string auth_token);
