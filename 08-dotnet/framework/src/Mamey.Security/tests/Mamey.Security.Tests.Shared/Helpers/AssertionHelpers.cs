using FluentAssertions;
using Shouldly;

namespace Mamey.Security.Tests.Shared.Helpers;

/// <summary>
/// Provides assertion helper utilities for testing.
/// </summary>
public static class AssertionHelpers
{
    /// <summary>
    /// Asserts that two encrypted values are different (encryption should produce different outputs).
    /// </summary>
    public static void ShouldBeEncrypted(string original, string encrypted)
    {
        encrypted.ShouldNotBe(original);
        encrypted.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Asserts that a value is a valid hash (hex string of expected length).
    /// </summary>
    public static void ShouldBeValidHash(string hash, int expectedLength = 128) // SHA-512 produces 128 hex chars
    {
        hash.ShouldNotBeNullOrEmpty();
        hash.Length.ShouldBe(expectedLength);
        hash.ShouldMatch("^[0-9a-f]+$");
    }

    /// <summary>
    /// Asserts that two hashes are the same for the same input.
    /// </summary>
    public static void ShouldBeConsistentHash(string hash1, string hash2)
    {
        hash1.ShouldBe(hash2, "Hashing the same input should produce the same output");
    }

    /// <summary>
    /// Asserts that a decrypted value matches the original.
    /// </summary>
    public static void ShouldDecryptToOriginal(string original, string decrypted)
    {
        decrypted.ShouldBe(original, "Decrypted value should match original");
    }

    /// <summary>
    /// Asserts that a signature is valid (base64 encoded).
    /// </summary>
    public static void ShouldBeValidSignature(string signature)
    {
        signature.ShouldNotBeNullOrEmpty();
        // Base64 validation
        try
        {
            Convert.FromBase64String(signature);
        }
        catch (FormatException)
        {
            throw new ShouldAssertException("Signature should be valid base64");
        }
    }

    /// <summary>
    /// Asserts that a random string is unique.
    /// </summary>
    public static void ShouldBeUnique(string value1, string value2)
    {
        value1.ShouldNotBe(value2, "Random values should be unique");
    }

    /// <summary>
    /// Asserts that a value is a valid encryption key length.
    /// </summary>
    public static void ShouldBeValidKeyLength(string key, int expectedLength)
    {
        key.ShouldNotBeNullOrEmpty();
        key.Length.ShouldBe(expectedLength, $"Key should be {expectedLength} characters long");
    }
}

