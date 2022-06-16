using Xunit;

namespace Helpers.RabbitMQ.Tests.CollectionDefinitions
{
	[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
	public class NonParallelCollectionDefinitionClass { }
}
