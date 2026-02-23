using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd;

/// <summary>
/// Collection definition for end-to-end tests to ensure they run sequentially
/// and don't compete for resources (testcontainers, WebApplicationFactory, etc.)
/// </summary>
[CollectionDefinition("EndToEnd", DisableParallelization = true)]
public class EndToEndTestCollection
{
    // This class is used only to define the collection
    // No implementation needed
}

