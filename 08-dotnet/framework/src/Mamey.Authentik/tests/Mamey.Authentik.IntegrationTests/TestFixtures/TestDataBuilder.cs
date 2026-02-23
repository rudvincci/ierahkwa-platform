using Bogus;

namespace Mamey.Authentik.IntegrationTests.TestFixtures;

/// <summary>
/// Builder for creating test data.
/// </summary>
public class TestDataBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// Creates a random username.
    /// </summary>
    public string CreateUsername() => _faker.Internet.UserName();

    /// <summary>
    /// Creates a random email address.
    /// </summary>
    public string CreateEmail() => _faker.Internet.Email();

    /// <summary>
    /// Creates a random name.
    /// </summary>
    public string CreateName() => _faker.Name.FullName();

    /// <summary>
    /// Creates a random group name.
    /// </summary>
    public string CreateGroupName() => $"TestGroup_{_faker.Random.AlphaNumeric(8)}";

    /// <summary>
    /// Creates a random application name.
    /// </summary>
    public string CreateApplicationName() => $"TestApp_{_faker.Random.AlphaNumeric(8)}";
}
