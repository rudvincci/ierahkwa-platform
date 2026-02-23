using Xunit;

namespace Mamey.Authentik.IntegrationTests;

/// <summary>
/// Collection definition for Authentik integration tests.
/// </summary>
[CollectionDefinition("AuthentikIntegration")]
public class AuthentikIntegrationCollection : ICollectionFixture<TestFixtures.AuthentikTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
