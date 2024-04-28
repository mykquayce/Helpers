namespace Xunit;

[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass { }
