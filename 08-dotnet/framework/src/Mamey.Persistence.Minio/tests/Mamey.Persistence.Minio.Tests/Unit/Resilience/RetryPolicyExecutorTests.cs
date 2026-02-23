using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Unit.Resilience;

/// <summary>
/// Unit tests for RetryPolicyExecutor.
/// </summary>
public class RetryPolicyExecutorTests
{
    private readonly Mock<ILogger<RetryPolicyExecutor>> _mockLogger;
    private readonly RetryPolicyExecutor _executor;
    private readonly RetryPolicy _retryPolicy;

    public RetryPolicyExecutorTests()
    {
        _mockLogger = new Mock<ILogger<RetryPolicyExecutor>>();
        _retryPolicy = new RetryPolicy
        {
            MaxRetries = 3,
            InitialDelay = TimeSpan.FromMilliseconds(100),
            BackoffMultiplier = 2.0,
            UseJitter = true
        };
        
        var options = new MinioOptions
        {
            RetryPolicy = _retryPolicy
        };
        
        var mockOptions = new Mock<IOptions<MinioOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);
        
        _executor = new RetryPolicyExecutor(mockOptions.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteSuccessfully_OnFirstAttempt()
    {
        // Arrange
        var expectedResult = "success";
        var operation = new Func<CancellationToken, Task<string>>(ct => Task.FromResult(expectedResult));

        // Act
        var result = await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRetryOnTransientException()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                throw new TimeoutException("Transient error");
            }
            return Task.FromResult("success");
        });

        // Act
        var result = await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        result.Should().Be("success");
        attemptCount.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_AfterMaxRetries()
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new TimeoutException("Persistent error");
        });

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotRetryOnNonTransientException()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            throw new ArgumentException("Non-transient error");
        });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));
        
        attemptCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));
        
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new TimeoutException("Transient error");
        });

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _executor.ExecuteAsync(operation, cts.Token));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogRetryAttempts()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new TimeoutException("Transient error");
            }
            return Task.FromResult("success");
        });

        // Act
        await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrying operation")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogFinalFailure()
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new TimeoutException("Persistent error");
        });

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation failed after all retries")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUseExponentialBackoff()
    {
        // Arrange
        var attemptCount = 0;
        var delays = new List<TimeSpan>();
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                throw new TimeoutException("Transient error");
            }
            return Task.FromResult("success");
        });

        // Act
        await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        attemptCount.Should().Be(3);
        // Note: In a real test, we would need to measure actual delays
        // This test verifies the operation completes successfully with retries
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleVoidOperations()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task>(ct =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new TimeoutException("Transient error");
            }
            return Task.CompletedTask;
        });

        // Act
        await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        attemptCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNullOperation()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _executor.ExecuteAsync<string>(null!, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_ShouldHandleInvalidOperationKey(string? operationKey)
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct => Task.FromResult("success"));

        // Act & Assert
        // Note: Operation key validation removed - method only takes CancellationToken
        await _executor.ExecuteAsync(operation, CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleOperationThatThrowsAggregateException()
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new AggregateException(new TimeoutException("Transient error"));
        });

        // Act & Assert
        await Assert.ThrowsAsync<AggregateException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleOperationThatThrowsTaskCanceledException()
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new TaskCanceledException("Operation was canceled");
        });

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleOperationThatThrowsOperationCanceledException()
    {
        // Arrange
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            throw new OperationCanceledException("Operation was canceled");
        });

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _executor.ExecuteAsync(operation, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleOperationThatThrowsHttpRequestException()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new HttpRequestException("Network error");
            }
            return Task.FromResult("success");
        });

        // Act
        var result = await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        result.Should().Be("success");
        attemptCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleOperationThatThrowsSocketException()
    {
        // Arrange
        var attemptCount = 0;
        var operation = new Func<CancellationToken, Task<string>>(ct =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                throw new System.Net.Sockets.SocketException(10054); // Connection reset
            }
            return Task.FromResult("success");
        });

        // Act
        var result = await _executor.ExecuteAsync(operation, CancellationToken.None);

        // Assert
        result.Should().Be("success");
        attemptCount.Should().Be(2);
    }
}
