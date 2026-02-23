namespace Mamey.Security.Tests.Shared.Helpers;

/// <summary>
/// Provides test data generation utilities.
/// </summary>
public static class TestDataGenerator
{
    /// <summary>
    /// Generates a random string of specified length.
    /// </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Generates a random byte array of specified length.
    /// </summary>
    public static byte[] GenerateRandomBytes(int length)
    {
        var bytes = new byte[length];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }

    /// <summary>
    /// Generates a large string (1MB).
    /// </summary>
    public static string GenerateLargeString()
    {
        return GenerateRandomString(1024 * 1024);
    }

    /// <summary>
    /// Generates a Unicode string with special characters.
    /// </summary>
    public static string GenerateUnicodeString()
    {
        return "Hello 世界 🌍 Привет こんにちは مرحبا";
    }

    /// <summary>
    /// Generates test data with special characters.
    /// </summary>
    public static string GenerateStringWithSpecialChars()
    {
        return "Test!@#$%^&*()_+-=[]{}|;':\",./<>?`~";
    }

    /// <summary>
    /// Generates an empty string.
    /// </summary>
    public static string GenerateEmptyString() => string.Empty;

    /// <summary>
    /// Generates test object for serialization.
    /// </summary>
    public static TestObject GenerateTestObject()
    {
        return new TestObject
        {
            Id = Guid.NewGuid(),
            Name = GenerateRandomString(20),
            Email = $"{GenerateRandomString(10)}@test.com",
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Test object for serialization tests.
/// </summary>
public class TestObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}



