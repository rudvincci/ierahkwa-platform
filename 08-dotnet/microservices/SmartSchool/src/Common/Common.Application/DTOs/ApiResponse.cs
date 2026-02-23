namespace Common.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse SuccessResponse(string? message = null) =>
        new() { Success = true, Message = message };

    public static ApiResponse FailureResponse(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
