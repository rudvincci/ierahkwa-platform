using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Options;
using Mamey.Auth.Decentralized.Persistence.Read;
using Mamey.Auth.Decentralized.Persistence.Read.Repositories;
using Mamey.Auth.Decentralized.Persistence.Write;
using Mamey.Auth.Decentralized.Validation;
using Mamey.Auth.Decentralized.Caching;
using Mamey.Auth.Decentralized.Tests.TestData;
using Mamey.Auth.Decentralized.Tests.TestConfiguration;

namespace Mamey.Auth.Decentralized.Tests.TestBase;

/// <summary>
/// Base class for unit tests providing common test functionality.
/// </summary>
public abstract class TestBase
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected IDecentralizedHandler DecentralizedHandler { get; private set; } = null!;
    protected Mamey.Auth.Decentralized.Validation.DidDocumentValidator DidDocumentValidator { get; private set; } = null!;
    protected Mamey.Auth.Decentralized.Validation.W3cComplianceValidator W3cComplianceValidator { get; private set; } = null!;
    protected IDidDocumentCache DidDocumentCache { get; private set; } = null!;
    protected IDidUnitOfWork DidUnitOfWork { get; private set; } = null!;
    protected IDidDocumentReadRepository DidDocumentReadRepository { get; private set; } = null!;
    protected IVerificationMethodReadRepository VerificationMethodReadRepository { get; private set; } = null!;
    protected IServiceEndpointReadRepository ServiceEndpointReadRepository { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;

    /// <summary>
    /// Initializes the test environment.
    /// </summary>
    // Removed [TestInitialize] - converting from MSTest to xUnit
    public virtual void TestInitialize()
    {
        var services = TestConfiguration.ConfigureTestServices();
        ServiceProvider = services.BuildServiceProvider();

        // Get required services
        DecentralizedHandler = ServiceProvider.GetRequiredService<IDecentralizedHandler>();
        DidDocumentValidator = ServiceProvider.GetRequiredService<IDidDocumentValidator>();
        W3cComplianceValidator = ServiceProvider.GetRequiredService<IW3cComplianceValidator>();
        DidDocumentCache = ServiceProvider.GetRequiredService<IDidDocumentCache>();
        DidUnitOfWork = ServiceProvider.GetRequiredService<IDidUnitOfWork>();
        DidDocumentReadRepository = ServiceProvider.GetRequiredService<IDidDocumentReadRepository>();
        VerificationMethodReadRepository = ServiceProvider.GetRequiredService<IVerificationMethodReadRepository>();
        ServiceEndpointReadRepository = ServiceProvider.GetRequiredService<IServiceEndpointReadRepository>();
        Logger = ServiceProvider.GetRequiredService<ILogger<TestBase>>();
    }

    /// <summary>
    /// Cleans up the test environment.
    /// </summary>
    [TestCleanup]
    public virtual void TestCleanup()
    {
        ServiceProvider?.Dispose();
    }

    /// <summary>
    /// Asserts that a DID is valid.
    /// </summary>
    /// <param name="did">The DID to validate.</param>
    protected void AssertValidDid(string did)
    {
        Assert.IsTrue(DidValidator.IsValidDid(did), $"DID '{did}' should be valid");
    }

    /// <summary>
    /// Asserts that a DID is invalid.
    /// </summary>
    /// <param name="did">The DID to validate.</param>
    protected void AssertInvalidDid(string did)
    {
        Assert.IsFalse(DidValidator.IsValidDid(did), $"DID '{did}' should be invalid");
    }

    /// <summary>
    /// Asserts that a DID URL is valid.
    /// </summary>
    /// <param name="didUrl">The DID URL to validate.</param>
    protected void AssertValidDidUrl(string didUrl)
    {
        Assert.IsTrue(DidValidator.IsValidDidUrl(didUrl), $"DID URL '{didUrl}' should be valid");
    }

    /// <summary>
    /// Asserts that a DID URL is invalid.
    /// </summary>
    /// <param name="didUrl">The DID URL to validate.</param>
    protected void AssertInvalidDidUrl(string didUrl)
    {
        Assert.IsFalse(DidValidator.IsValidDidUrl(didUrl), $"DID URL '{didUrl}' should be invalid");
    }

    /// <summary>
    /// Asserts that a DID document is valid.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    protected async Task AssertValidDidDocumentAsync(DidDocument didDocument)
    {
        var result = await DidDocumentValidator.ValidateAsync(didDocument);
        Assert.IsTrue(result.IsValid, $"DID document should be valid. Errors: {string.Join(", ", result.Errors)}");
    }

    /// <summary>
    /// Asserts that a DID document is invalid.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    protected async Task AssertInvalidDidDocumentAsync(DidDocument didDocument)
    {
        var result = await DidDocumentValidator.ValidateAsync(didDocument);
        Assert.IsFalse(result.IsValid, "DID document should be invalid");
    }

    /// <summary>
    /// Asserts that a DID document is W3C compliant.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    protected async Task AssertW3cCompliantDidDocumentAsync(DidDocument didDocument)
    {
        var result = await W3cComplianceValidator.ValidateAsync(didDocument);
        Assert.IsTrue(result.IsCompliant, $"DID document should be W3C compliant. Errors: {string.Join(", ", result.Errors)}");
    }

    /// <summary>
    /// Asserts that a DID document is not W3C compliant.
    /// </summary>
    /// <param name="didDocument">The DID document to validate.</param>
    protected async Task AssertNonW3cCompliantDidDocumentAsync(DidDocument didDocument)
    {
        var result = await W3cComplianceValidator.ValidateAsync(didDocument);
        Assert.IsFalse(result.IsCompliant, "DID document should not be W3C compliant");
    }

    /// <summary>
    /// Asserts that a DID resolution is successful.
    /// </summary>
    /// <param name="did">The DID to resolve.</param>
    protected async Task AssertSuccessfulDidResolutionAsync(string did)
    {
        var result = await DecentralizedHandler.ResolveDidAsync(did);
        Assert.IsTrue(result.IsSuccessful, $"DID resolution should be successful for '{did}'. Error: {result.Error}");
        Assert.IsNotNull(result.DidDocument, "DID document should not be null");
    }

    /// <summary>
    /// Asserts that a DID resolution fails.
    /// </summary>
    /// <param name="did">The DID to resolve.</param>
    protected async Task AssertFailedDidResolutionAsync(string did)
    {
        var result = await DecentralizedHandler.ResolveDidAsync(did);
        Assert.IsFalse(result.IsSuccessful, $"DID resolution should fail for '{did}'");
        Assert.IsNull(result.DidDocument, "DID document should be null");
    }

    /// <summary>
    /// Asserts that an exception is thrown.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected.</typeparam>
    /// <param name="action">The action that should throw the exception.</param>
    protected void AssertThrowsException<TException>(Action action) where TException : Exception
    {
        Assert.ThrowsException<TException>(action);
    }

    /// <summary>
    /// Asserts that an exception is thrown asynchronously.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected.</typeparam>
    /// <param name="action">The action that should throw the exception.</param>
    protected async Task AssertThrowsExceptionAsync<TException>(Func<Task> action) where TException : Exception
    {
        await Assert.ThrowsExceptionAsync<TException>(action);
    }

    /// <summary>
    /// Asserts that two DIDs are equal.
    /// </summary>
    /// <param name="expected">The expected DID.</param>
    /// <param name="actual">The actual DID.</param>
    protected void AssertDidEquals(string expected, string actual)
    {
        Assert.AreEqual(expected, actual, "DIDs should be equal");
    }

    /// <summary>
    /// Asserts that two DID URLs are equal.
    /// </summary>
    /// <param name="expected">The expected DID URL.</param>
    /// <param name="actual">The actual DID URL.</param>
    protected void AssertDidUrlEquals(string expected, string actual)
    {
        Assert.AreEqual(expected, actual, "DID URLs should be equal");
    }

    /// <summary>
    /// Asserts that two DID documents are equal.
    /// </summary>
    /// <param name="expected">The expected DID document.</param>
    /// <param name="actual">The actual DID document.</param>
    protected void AssertDidDocumentEquals(DidDocument expected, DidDocument actual)
    {
        Assert.AreEqual(expected.Id, actual.Id, "DID document IDs should be equal");
        Assert.AreEqual(expected.Controller, actual.Controller, "DID document controllers should be equal");
        Assert.AreEqual(expected.VerificationMethod.Count, actual.VerificationMethod.Count, "DID document verification methods count should be equal");
        Assert.AreEqual(expected.Service?.Count ?? 0, actual.Service?.Count ?? 0, "DID document services count should be equal");
    }

    /// <summary>
    /// Asserts that a collection is not empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertCollectionNotEmpty<T>(IEnumerable<T> collection, string message = "Collection should not be empty")
    {
        Assert.IsTrue(collection.Any(), message);
    }

    /// <summary>
    /// Asserts that a collection is empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertCollectionEmpty<T>(IEnumerable<T> collection, string message = "Collection should be empty")
    {
        Assert.IsFalse(collection.Any(), message);
    }

    /// <summary>
    /// Asserts that a string is not null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringNotEmpty(string? value, string message = "String should not be null or empty")
    {
        Assert.IsFalse(string.IsNullOrEmpty(value), message);
    }

    /// <summary>
    /// Asserts that a string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringEmpty(string? value, string message = "String should be null or empty")
    {
        Assert.IsTrue(string.IsNullOrEmpty(value), message);
    }

    /// <summary>
    /// Asserts that a value is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertNotNull<T>(T? value, string message = "Value should not be null")
    {
        Assert.IsNotNull(value, message);
    }

    /// <summary>
    /// Asserts that a value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertNull<T>(T? value, string message = "Value should be null")
    {
        Assert.IsNull(value, message);
    }

    /// <summary>
    /// Asserts that a boolean value is true.
    /// </summary>
    /// <param name="value">The boolean value to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertTrue(bool value, string message = "Value should be true")
    {
        Assert.IsTrue(value, message);
    }

    /// <summary>
    /// Asserts that a boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to check.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertFalse(bool value, string message = "Value should be false")
    {
        Assert.IsFalse(value, message);
    }

    /// <summary>
    /// Asserts that two values are equal.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertEquals<T>(T expected, T actual, string message = "Values should be equal")
    {
        Assert.AreEqual(expected, actual, message);
    }

    /// <summary>
    /// Asserts that two values are not equal.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertNotEquals<T>(T expected, T actual, string message = "Values should not be equal")
    {
        Assert.AreNotEqual(expected, actual, message);
    }

    /// <summary>
    /// Asserts that a value is within a range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertInRange(double value, double min, double max, string message = "Value should be within range")
    {
        Assert.IsTrue(value >= min && value <= max, $"{message}. Value: {value}, Range: [{min}, {max}]");
    }

    /// <summary>
    /// Asserts that a value is greater than a threshold.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertGreaterThan(double value, double threshold, string message = "Value should be greater than threshold")
    {
        Assert.IsTrue(value > threshold, $"{message}. Value: {value}, Threshold: {threshold}");
    }

    /// <summary>
    /// Asserts that a value is less than a threshold.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertLessThan(double value, double threshold, string message = "Value should be less than threshold")
    {
        Assert.IsTrue(value < threshold, $"{message}. Value: {value}, Threshold: {threshold}");
    }

    /// <summary>
    /// Asserts that a value is greater than or equal to a threshold.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertGreaterThanOrEqual(double value, double threshold, string message = "Value should be greater than or equal to threshold")
    {
        Assert.IsTrue(value >= threshold, $"{message}. Value: {value}, Threshold: {threshold}");
    }

    /// <summary>
    /// Asserts that a value is less than or equal to a threshold.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertLessThanOrEqual(double value, double threshold, string message = "Value should be less than or equal to threshold")
    {
        Assert.IsTrue(value <= threshold, $"{message}. Value: {value}, Threshold: {threshold}");
    }

    /// <summary>
    /// Asserts that a string contains a substring.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="substring">The substring to look for.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringContains(string value, string substring, string message = "String should contain substring")
    {
        Assert.IsTrue(value.Contains(substring), $"{message}. String: '{value}', Substring: '{substring}'");
    }

    /// <summary>
    /// Asserts that a string does not contain a substring.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="substring">The substring to look for.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringDoesNotContain(string value, string substring, string message = "String should not contain substring")
    {
        Assert.IsFalse(value.Contains(substring), $"{message}. String: '{value}', Substring: '{substring}'");
    }

    /// <summary>
    /// Asserts that a string starts with a prefix.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="prefix">The prefix to look for.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringStartsWith(string value, string prefix, string message = "String should start with prefix")
    {
        Assert.IsTrue(value.StartsWith(prefix), $"{message}. String: '{value}', Prefix: '{prefix}'");
    }

    /// <summary>
    /// Asserts that a string ends with a suffix.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="suffix">The suffix to look for.</param>
    /// <param name="message">The assertion message.</param>
    protected void AssertStringEndsWith(string value, string suffix, string message = "String should end with suffix")
    {
        Assert.IsTrue(value.EndsWith(suffix), $"{message}. String: '{value}', Suffix: '{suffix}'");
    }
}
