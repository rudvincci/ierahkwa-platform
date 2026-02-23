using Microsoft.Extensions.Logging;
using Moq;

namespace Mamey.Authentik.UnitTests;

/// <summary>
/// Base class for unit tests with common setup.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Creates a mock logger.
    /// </summary>
    protected static ILogger<T> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }

    /// <summary>
    /// Creates a mock logger factory.
    /// </summary>
    protected static ILoggerFactory CreateMockLoggerFactory()
    {
        return new Mock<ILoggerFactory>().Object;
    }
}
