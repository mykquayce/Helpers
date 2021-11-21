namespace Helpers.DockerHub.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public record AuthResponseObject(string token, string access_token, int expires_in, DateTime issued_at);
