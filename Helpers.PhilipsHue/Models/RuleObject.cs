namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record RuleObject(
	string name, string owner, DateTime created, string lasttriggered, int timestriggered,
	string status, bool recycle, IReadOnlyList<RuleObject.Condition> conditions, IReadOnlyList<RuleObject.Action> actions)
{
	public record Condition(string address, string @operator, string value);

	public record Action(string address, string method, Action.Body body)
	{
		public record Body(int bri_inc, bool on, string scene, object status, string time, int transitiontime);
	}
}
