using Mamey.Security.Tests.Shared.Fixtures;

namespace Mamey.Security.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for MongoDB integration tests.
/// </summary>
public class MongoDBTestFixture : SecurityTestFixture
{
    public MongoDBTestFixture(bool encryptionEnabled = true) : base(encryptionEnabled)
    {
    }
}

/// <summary>
/// Test document for MongoDB tests.
/// </summary>
public class TestDocument
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EncryptedProperty { get; set; }
    public string? HashedProperty { get; set; }
}



