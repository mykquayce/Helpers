namespace Xunit;

[CollectionDefinition(nameof(NonParallelCollection), DisableParallelization = true)]
public class NonParallelCollection { }
