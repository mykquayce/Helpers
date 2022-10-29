namespace Helpers.GlobalCache.Exceptions;

public class UnexpectedResponseException : Exception
{
	public UnexpectedResponseException(string request, string response)
		: base($"unexpected {nameof(response)}: {response}")
	{
		Data.Add(nameof(request), request);
		Data.Add(nameof(response), response);
	}
}
