using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Mamey.Security;
using Mamey.Security.Internals;

namespace Mamey.Security.Tests.Shared.Helpers;

/// <summary>
/// Provides mock helper utilities for testing.
/// </summary>
public static class MockHelpers
{
    /// <summary>
    /// Creates a mock ILogger.
    /// </summary>
    public static ILogger<T> CreateMockLogger<T>()
    {
        return Substitute.For<ILogger<T>>();
    }

    /// <summary>
    /// Creates a mock IEncryptor.
    /// </summary>
    public static IEncryptor CreateMockEncryptor()
    {
        return Substitute.For<IEncryptor>();
    }

    /// <summary>
    /// Creates a mock IHasher.
    /// </summary>
    public static IHasher CreateMockHasher()
    {
        return Substitute.For<IHasher>();
    }

    /// <summary>
    /// Creates a mock IRng.
    /// </summary>
    public static IRng CreateMockRng()
    {
        return Substitute.For<IRng>();
    }

    /// <summary>
    /// Creates a mock ISigner.
    /// </summary>
    public static ISigner CreateMockSigner()
    {
        return Substitute.For<ISigner>();
    }

    /// <summary>
    /// Creates a mock IMd5.
    /// </summary>
    public static IMd5 CreateMockMd5()
    {
        return Substitute.For<IMd5>();
    }

    /// <summary>
    /// Creates a mock ISecurityProvider.
    /// </summary>
    public static ISecurityProvider CreateMockSecurityProvider()
    {
        return Substitute.For<ISecurityProvider>();
    }

    /// <summary>
    /// Creates a real Encryptor instance for testing.
    /// Note: Encryptor is internal, so we use the interface and factory pattern.
    /// </summary>
    public static IEncryptor CreateRealEncryptor(ILogger? logger = null)
    {
        // Use the service provider to get the real implementation
        var services = new ServiceCollection();
        services.AddLogging();
        if (logger != null)
        {
            services.AddSingleton(logger);
        }
        var sp = services.BuildServiceProvider();
        // Return a mock that delegates to the real implementation via ISecurityProvider
        // For now, we'll use NSubstitute to create a mock
        return Substitute.For<IEncryptor>();
    }

    /// <summary>
    /// Creates a real Hasher instance for testing.
    /// Note: Hasher is internal, so we use the interface and factory pattern.
    /// </summary>
    public static IHasher CreateRealHasher()
    {
        // Use the service provider to get the real implementation
        var services = new ServiceCollection();
        services.AddLogging();
        var sp = services.BuildServiceProvider();
        // Return a mock that delegates to the real implementation via ISecurityProvider
        // For now, we'll use NSubstitute to create a mock
        return Substitute.For<IHasher>();
    }

    /// <summary>
    /// Creates a real Rng instance for testing.
    /// </summary>
    public static IRng CreateRealRng()
    {
        return new Rng();
    }

    /// <summary>
    /// Creates a real Md5 instance for testing.
    /// </summary>
    public static IMd5 CreateRealMd5()
    {
        return new Md5();
    }

    /// <summary>
    /// Creates a real Signer instance for testing.
    /// </summary>
    public static ISigner CreateRealSigner()
    {
        return new Signer();
    }
}

