using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Tests.Shared.Factories;

/// <summary>
/// Factory for creating test data for Identities service tests.
/// </summary>
public static class TestDataFactory
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Creates a new IdentityId.
    /// </summary>
    public static IdentityId CreateIdentityId()
    {
        return new IdentityId(Guid.NewGuid());
    }

    /// <summary>
    /// Creates a random BiometricType.
    /// </summary>
    public static BiometricType CreateBiometricType()
    {
        var types = Enum.GetValues<BiometricType>();
        return types[_random.Next(types.Length)];
    }

    /// <summary>
    /// Creates biometric data bytes of the specified size.
    /// </summary>
    public static byte[] CreateBiometricData(int size = 1024)
    {
        var data = new byte[size];
        _random.NextBytes(data);
        return data;
    }

    /// <summary>
    /// Creates an object name for biometric data storage.
    /// </summary>
    public static string CreateObjectName(IdentityId identityId, BiometricType type)
    {
        return $"biometric-data/{identityId.Value}/{type.ToString().ToLowerInvariant()}.bin";
    }

    /// <summary>
    /// Creates a test Identity with default values.
    /// </summary>
    public static Identity CreateTestIdentity(
        IdentityId? id = null,
        string? firstName = null,
        string? lastName = null,
        string? middleName = null,
        DateTime? dateOfBirth = null,
        string? placeOfBirth = null,
        string? gender = null,
        string? clan = null,
        string? email = null,
        string? zone = null,
        Guid? clanRegistrarId = null)
    {
        id ??= CreateIdentityId();
        firstName ??= "John";
        lastName ??= "Doe";
        middleName ??= "M.";
        dateOfBirth ??= new DateTime(1990, 1, 1);
        placeOfBirth ??= "New York, NY";
        gender ??= "Male";
        clan ??= "Wolf Clan";
        email ??= $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}@example.com";
        zone ??= "zone-001";

        var name = new Name(firstName, lastName, middleName);
        var personalDetails = new PersonalDetails(dateOfBirth.Value, placeOfBirth, gender, clan);
        var contactInfo = new ContactInformation(
            new Email(email),
            new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
        );
        // Create biometric data with a realistic hash (128-char hex string)
        // to simulate what SecurityAttributeProcessor would produce
        var biometricBytes = CreateBiometricData();
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var biometricData = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        return new Identity(id, name, personalDetails, contactInfo, biometricData, zone, clanRegistrarId);
    }

    /// <summary>
    /// Creates a list of test identities.
    /// </summary>
    public static List<Identity> CreateTestIdentities(int count, string? zone = null)
    {
        var identities = new List<Identity>();
        var firstNames = new[] { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Eve", "Frank", "Grace", "Henry" };
        var lastNames = new[] { "Doe", "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Wilson" };
        var clans = new[] { "Wolf Clan", "Bear Clan", "Eagle Clan", "Turtle Clan", "Deer Clan" };
        var zones = new[] { "zone-001", "zone-002", "zone-003", "zone-004", "zone-005" };

        for (int i = 0; i < count; i++)
        {
            var identity = CreateTestIdentity(
                firstName: firstNames[i % firstNames.Length],
                lastName: lastNames[i % lastNames.Length],
                middleName: i % 2 == 0 ? "M." : null,
                dateOfBirth: new DateTime(1990 + (i % 20), 1 + (i % 12), 1 + (i % 28)),
                placeOfBirth: $"City {i % 10}, State",
                gender: i % 2 == 0 ? "Male" : "Female",
                clan: clans[i % clans.Length],
                email: $"user{i}@example.com",
                zone: zone ?? zones[i % zones.Length]
            );
            identities.Add(identity);
        }

        return identities;
    }

    /// <summary>
    /// Creates a test BiometricData value object.
    /// Note: The Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
    /// when persisted. This factory creates a hash that simulates what would be stored (128-char hex string).
    /// </summary>
    public static BiometricData CreateTestBiometricData(BiometricType? type = null, int dataSize = 1024, string? hash = null)
    {
        type ??= CreateBiometricType();
        var data = CreateBiometricData(dataSize);
        
        // If hash is provided, use it; otherwise generate a 128-character hex string
        // to simulate what SecurityAttributeProcessor would produce (SHA-512 hash)
        if (string.IsNullOrEmpty(hash))
        {
            // Generate a 128-character hex string to simulate SHA-512 hash
            // (SecurityAttributeProcessor produces 128-char hex strings)
            var hashBytes = System.Security.Cryptography.SHA512.HashData(data);
            hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        
        return new BiometricData(type.Value, data, hash);
    }
    
    /// <summary>
    /// Creates a BiometricData with a specific hash value for testing matching logic.
    /// Use this when you need to test hash comparison behavior.
    /// Note: Hash should be a 128-character hex string to simulate what SecurityAttributeProcessor produces.
    /// </summary>
    public static BiometricData CreateTestBiometricDataWithHash(BiometricType type, byte[] data, string hash)
    {
        return new BiometricData(type, data, hash);
    }
    
    /// <summary>
    /// Creates a 128-character hex string hash to simulate what SecurityAttributeProcessor produces.
    /// This is useful for creating test data that matches what would be stored in the database.
    /// </summary>
    public static string CreateHashedValue(string plainText)
    {
        var hashBytes = System.Security.Cryptography.SHA512.HashData(System.Text.Encoding.UTF8.GetBytes(plainText));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    /// <summary>
    /// Creates a 128-character hex string hash from byte data.
    /// This simulates what SecurityAttributeProcessor produces for the Hash property.
    /// </summary>
    public static string CreateHashedValueFromBytes(byte[] data)
    {
        var hashBytes = System.Security.Cryptography.SHA512.HashData(data);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Creates a test ContactInformation value object.
    /// </summary>
    public static ContactInformation CreateTestContactInformation(string? email = null, string? phone = null)
    {
        email ??= "test@example.com";
        phone ??= "5551234567";

        return new ContactInformation(
            new Email(email),
            new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
            new List<Phone> { new Phone("1", phone, null, Phone.PhoneType.Mobile) }
        );
    }

    /// <summary>
    /// Creates a test PersonalDetails value object.
    /// </summary>
    public static PersonalDetails CreateTestPersonalDetails(
        DateTime? dateOfBirth = null,
        string? placeOfBirth = null,
        string? gender = null,
        string? clan = null)
    {
        dateOfBirth ??= new DateTime(1990, 1, 1);
        placeOfBirth ??= "New York, NY";
        gender ??= "Male";
        clan ??= "Wolf Clan";

        return new PersonalDetails(dateOfBirth.Value, placeOfBirth, gender, clan);
    }
}

