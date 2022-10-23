using static Dawn.Guard;

namespace Dawn;

public static class ExtensionMethods
{
	public static ref readonly ArgumentInfo<string> LeadingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} must have leading white-space";
		return ref argument.Matches(@"^\s+", (s, b) => message);
	}

	public static ref readonly ArgumentInfo<string> NoLeadingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} mustn't have leading white-space";
		return ref argument.Matches(@"^\S+", (s, b) => message);
	}

	public static ref readonly ArgumentInfo<string> TrailingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} must have trailing white-space";
		return ref argument.Matches(@"\s+$", (s, b) => message);
	}

	public static ref readonly ArgumentInfo<string> NoTrailingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} mustn't have trailing white-space";
		return ref argument.Matches(@"\S+$", (s, b) => message);
	}

	public static ref readonly ArgumentInfo<string> LeadingAndTrailingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} must have leading and trailing white-space";
		return ref argument.Matches(@"^\s+\S+\s+$", (s, b) => message);
	}

	public static ref readonly ArgumentInfo<string> NoLeadingOrTrailingWhiteSpace(in this ArgumentInfo<string> argument)
	{
		return ref argument
			.NoLeadingWhiteSpace()
			.NoTrailingWhiteSpace();
	}
}
