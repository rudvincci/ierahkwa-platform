using System.Reflection;

namespace Mamey.Security;

/// <summary>
/// Service for processing objects and applying encryption/hashing based on [EncryptedAttribute] and [HashedAttribute] attributes.
/// </summary>
public class SecurityAttributeProcessor
{
    private readonly ISecurityProvider _securityProvider;

    public SecurityAttributeProcessor(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    /// <summary>
    /// Processes an object and encrypts properties marked with [EncryptedAttribute].
    /// </summary>
    /// <param name="obj">The object to process.</param>
    /// <param name="direction">The processing direction (ToStorage = encrypt, FromStorage = decrypt).</param>
    public void ProcessEncryptedProperties(object obj, ProcessingDirection direction = ProcessingDirection.ToStorage)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<EncryptedAttribute>() == null)
                continue;

            var value = property.GetValue(obj) as string;
            if (value == null)
                continue;

            var processedValue = direction == ProcessingDirection.ToStorage
                ? _securityProvider.Encrypt(value)
                : _securityProvider.Decrypt(value);

            property.SetValue(obj, processedValue);
        }
    }

    /// <summary>
    /// Processes an object and hashes properties marked with [HashedAttribute].
    /// Note: Hashing is one-way, so this only applies hashing (not reverse).
    /// </summary>
    /// <param name="obj">The object to process.</param>
    public void ProcessHashedProperties(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<HashedAttribute>() == null)
                continue;

            var value = property.GetValue(obj) as string;
            if (string.IsNullOrEmpty(value))
                continue;

            // Only hash if the value doesn't look like it's already hashed
            // (SHA-512 produces 128 character hex strings)
            if (value.Length == 128 && value.All(c => char.IsLetterOrDigit(c)))
            {
                // Already hashed, skip
                continue;
            }

            var hashedValue = _securityProvider.Hash(value);
            property.SetValue(obj, hashedValue);
        }
    }

    /// <summary>
    /// Processes an object and applies both encryption and hashing based on attributes.
    /// </summary>
    /// <param name="obj">The object to process.</param>
    /// <param name="direction">The processing direction for encrypted properties.</param>
    public void ProcessAllSecurityAttributes(object obj, ProcessingDirection direction = ProcessingDirection.ToStorage)
    {
        ProcessEncryptedProperties(obj, direction);
        ProcessHashedProperties(obj);
    }

    /// <summary>
    /// Verifies a hashed property value against a plain text value.
    /// </summary>
    /// <param name="obj">The object containing the hashed property.</param>
    /// <param name="propertyName">The name of the property to verify.</param>
    /// <param name="plainTextValue">The plain text value to verify against.</param>
    /// <returns>True if the plain text value matches the hashed property value.</returns>
    public bool VerifyHashedProperty(object obj, string propertyName, string plainTextValue)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

        var type = obj.GetType();
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{type.Name}'.", nameof(propertyName));

        if (property.GetCustomAttribute<HashedAttribute>() == null)
            throw new ArgumentException($"Property '{propertyName}' is not marked with [HashedAttribute].", nameof(propertyName));

        var hashedValue = property.GetValue(obj) as string;
        if (string.IsNullOrEmpty(hashedValue))
            return false;

        var computedHash = _securityProvider.Hash(plainTextValue);
        return string.Equals(hashedValue, computedHash, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Direction for processing encrypted properties.
/// </summary>
public enum ProcessingDirection
{
    /// <summary>
    /// Process for storage (encrypt).
    /// </summary>
    ToStorage,

    /// <summary>
    /// Process from storage (decrypt).
    /// </summary>
    FromStorage
}



