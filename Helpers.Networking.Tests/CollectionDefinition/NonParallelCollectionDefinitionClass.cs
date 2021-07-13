using Xunit;

namespace Helpers.Networking.Tests.CollectionDefinition
{
	[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
	public class NonParallelCollectionDefinitionClass { }
}
