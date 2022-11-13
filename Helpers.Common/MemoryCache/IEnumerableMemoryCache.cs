namespace Microsoft.Extensions.Caching.Memory;

public interface IEnumerableMemoryCache : IMemoryCache, IDictionary<object, object?>
{ }
