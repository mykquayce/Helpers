namespace System;

public static class ExceptionExtensions
{
	public static IEnumerable<string> GetMessages(this Exception exception)
		=> exception.GetExceptions().Select(ex => ex.Message);

	public static IEnumerable<Exception> GetExceptions(this Exception exception)
	{
		yield return exception;

		if (exception is AggregateException aggregateException)
		{
			foreach (var inner in aggregateException.InnerExceptions)
			{
				yield return inner;
			}
		}

		if (exception.InnerException is not null)
		{
			yield return exception.InnerException;
		}
	}
}
