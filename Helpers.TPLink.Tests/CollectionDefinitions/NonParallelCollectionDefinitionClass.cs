using Xunit;

namespace Helpers.TPLink.Tests.CollectionDefinitions
{
	[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
	public class NonParallelCollectionDefinitionClass
	{ }
}
