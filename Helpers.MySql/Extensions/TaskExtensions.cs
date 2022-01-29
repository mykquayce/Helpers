namespace System.Threading.Tasks;

public static class TaskExtensions
{
	public async static Task<T> SafeAwaitAsync<T>(this Task<T> task)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 30_000);

		while (!cts.IsCancellationRequested)
		{
			try
			{
				return await task;
			}
			catch (Exception ex) when (ex.Message == "Unable to connect to any of the specified MySQL hosts.")
			{ }
			catch (Exception ex) when (ex.Message == "Reading from the stream has failed.")
			{ }
			catch
			{
				throw;
			}

			await Task.Delay(millisecondsDelay: 3_000);
		}

		throw new Helpers.MySql.Exceptions.ConnectionFailedException();
	}
}
