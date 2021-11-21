namespace Helpers.DockerHub.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public record TagsResponseObject(string name, IReadOnlyCollection<string> tags);
