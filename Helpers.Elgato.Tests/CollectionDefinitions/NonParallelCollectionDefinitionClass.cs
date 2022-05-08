using Xunit;

namespace Helpers.Elgato.Tests.CollectionDefinitions;

[CollectionDefinition(nameof(NonParallelCollectionDefinitionClass), DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass { }
