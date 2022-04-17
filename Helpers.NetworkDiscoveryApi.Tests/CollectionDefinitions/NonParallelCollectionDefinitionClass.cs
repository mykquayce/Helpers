using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests.CollectionDefinitions
{
	[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
	public class NonParallelCollectionDefinitionClass { }
}
