namespace MameyFramework.Core;

/// <summary>
/// Result pattern for handling success/failure without exceptions
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }
    
    protected Result(bool isSuccess, string? error = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }
    
    public static Result Success() => new(true);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    
    public static Result<T> Success<T>(T value) => new(value, true);
    public static Result<T> Failure<T>(string error, string? errorCode = null) => new(default!, false, error, errorCode);
}

/// <summary>
/// Result with value
/// </summary>
public class Result<T> : Result
{
    public T Value { get; }
    
    internal Result(T value, bool isSuccess, string? error = null, string? errorCode = null)
        : base(isSuccess, error, errorCode)
    {
        Value = value;
    }
    
    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Pagination result
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    
    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
