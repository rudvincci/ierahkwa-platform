using Mamey.Security.Tests.Shared.Fixtures;

namespace Mamey.Security.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for Redis integration tests.
/// </summary>
public class RedisTestFixture : SecurityTestFixture
{
    public RedisTestFixture(bool encryptionEnabled = true) : base(encryptionEnabled)
    {
    }
}



