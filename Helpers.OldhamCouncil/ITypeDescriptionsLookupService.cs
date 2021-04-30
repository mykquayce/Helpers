using System.Collections.Generic;

namespace Helpers.OldhamCouncil
{
	public interface ITypeDescriptionsLookupService<T> : IReadOnlyDictionary<string, T> { }
}
