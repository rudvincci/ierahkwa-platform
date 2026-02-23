using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration;

/// <summary>
/// Collection definition for integration tests to ensure they run sequentially
/// and don't compete for resources (testcontainers, etc.)
/// </summary>
[CollectionDefinition("Integration", DisableParallelization = true)]
public class IntegrationTestCollection
{
    // This class is used only to define the collection
    // No implementation needed
}

