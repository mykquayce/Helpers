namespace Helpers.PhilipsHue.Exceptions;

public class DeserializationException<T> : Exception
{
	public DeserializationException(string requestUri)
		: base(message: $"failed to deserialze response from {requestUri} as {typeof(T).FullName}")
	{
		base.Data.Add(nameof(requestUri), requestUri);
		base.Data.Add(nameof(T), typeof(T).FullName);
	}
}
