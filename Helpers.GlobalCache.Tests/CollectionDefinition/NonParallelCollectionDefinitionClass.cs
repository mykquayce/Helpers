namespace Helpers.GlobalCache.Tests.CollectionDefinition;

[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass { }
