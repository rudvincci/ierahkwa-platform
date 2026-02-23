using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Infrastructure.Resilience;

namespace Mamey.Persistence.Minio.Infrastructure;

/// <summary>
/// Base class for Minio services providing common functionality.
/// </summary>
public abstract class BaseMinioService
{
    protected readonly IMinioClient Client;
    protected readonly MinioOptions Options;
    protected readonly ILogger Logger;
    protected readonly IRetryPolicyExecutor RetryPolicyExecutor;

    protected BaseMinioService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger logger,
        IRetryPolicyExecutor retryPolicyExecutor)
    {
        Client = client;
        Options = options.Value;
        Logger = logger;
        RetryPolicyExecutor = retryPolicyExecutor;
    }

    /// <summary>
    /// Executes an operation with retry policy and error handling.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="operationName">The name of the operation for logging.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result.</returns>
    protected async Task<TResult> ExecuteWithRetryAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("Executing operation: {OperationName}", operationName);
            var result = await RetryPolicyExecutor.ExecuteAsync(operation, cancellationToken);
            Logger.LogDebug("Operation completed successfully: {OperationName}", operationName);
            return result;
        }
        catch (Exception ex) when (!(ex is MinioServiceException))
        {
            Logger.LogError(ex, "Operation failed: {OperationName}", operationName);
            throw CreateServiceException(operationName, ex);
        }
    }

    /// <summary>
    /// Executes an operation with retry policy and error handling.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="operationName">The name of the operation for logging.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result.</returns>
    protected async Task ExecuteWithRetryAsync(
        Func<CancellationToken, Task> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("Executing operation: {OperationName}", operationName);
            await RetryPolicyExecutor.ExecuteAsync(operation, cancellationToken);
            Logger.LogDebug("Operation completed successfully: {OperationName}", operationName);
        }
        catch (Exception ex) when (!(ex is MinioServiceException))
        {
            Logger.LogError(ex, "Operation failed: {OperationName}", operationName);
            throw CreateServiceException(operationName, ex);
        }
    }

    /// <summary>
    /// Creates an appropriate service exception based on the operation and error.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="exception">The original exception.</param>
    /// <returns>A MinioServiceException.</returns>
    protected virtual MinioServiceException CreateServiceException(string operationName, Exception exception)
    {
        return new MinioServiceException(
            $"Operation '{operationName}' failed",
            "operation_failed",
            $"An error occurred while executing '{operationName}'",
            exception);
    }

    /// <summary>
    /// Validates that a bucket name is not null or empty.
    /// </summary>
    /// <param name="bucketName">The bucket name to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    /// <exception cref="ArgumentException">Thrown when the bucket name is invalid.</exception>
    protected static void ValidateBucketName(string bucketName, string paramName = "bucketName")
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            throw new ArgumentException("Bucket name cannot be null or empty.", paramName);
    }

    /// <summary>
    /// Validates that an object name is not null or empty.
    /// </summary>
    /// <param name="objectName">The object name to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    /// <exception cref="ArgumentException">Thrown when the object name is invalid.</exception>
    protected static void ValidateObjectName(string objectName, string paramName = "objectName")
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty.", paramName);
    }

    /// <summary>
    /// Validates that both bucket and object names are not null or empty.
    /// </summary>
    /// <param name="bucketName">The bucket name to validate.</param>
    /// <param name="objectName">The object name to validate.</param>
    /// <exception cref="ArgumentException">Thrown when either name is invalid.</exception>
    protected static void ValidateBucketAndObjectNames(string bucketName, string objectName)
    {
        ValidateBucketName(bucketName);
        ValidateObjectName(objectName);
    }
}
