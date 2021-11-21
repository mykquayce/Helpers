namespace Helpers.DockerHub.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public record ManifestsResponseObject(
	int schemaVersion,
	string name,
	string tag,
	string architecture,
	IReadOnlyCollection<ManifestsResponseObject.Fslayer> fsLayers,
	IReadOnlyCollection<ManifestsResponseObject.History> history,
	IReadOnlyCollection<ManifestsResponseObject.Signature> signatures)
{
	public record Fslayer(string blobSum);
	public record History(string v1Compatibility);
	public record Signature(Signature.Header header, string signature, string _protected)
	{
		public record Header(Header.Jwk jwk, string alg)
		{
			public record Jwk(string crv, string kid, string kty, string x, string y);
		}
	}
}
