using Helpers.Elgato.Models;

namespace System.Text.Json.Serialization;

[JsonSerializable(typeof(Info))]
[JsonSerializable(typeof(Message<WhiteLight>))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
