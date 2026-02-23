namespace Pupitre.Contracts.DTOs;

/// <summary>
/// Standard API response wrapper.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public sealed record ApiResponse<T>
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The response data (if successful).
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Error message (if failed).
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Error code (if failed).
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Correlation ID for request tracing.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? correlationId = null)
        => new()
        {
            Success = true,
            Data = data,
            CorrelationId = correlationId
        };

    /// <summary>
    /// Creates a failed response with error details.
    /// </summary>
    public static ApiResponse<T> Fail(string error, string? errorCode = null, string? correlationId = null)
        => new()
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode,
            CorrelationId = correlationId
        };
}

/// <summary>
/// Non-generic API response for operations that don't return data.
/// </summary>
public sealed record ApiResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? ErrorCode { get; init; }
    public string? CorrelationId { get; init; }

    public static ApiResponse Ok(string? correlationId = null)
        => new() { Success = true, CorrelationId = correlationId };

    public static ApiResponse Fail(string error, string? errorCode = null, string? correlationId = null)
        => new() { Success = false, Error = error, ErrorCode = errorCode, CorrelationId = correlationId };
}
