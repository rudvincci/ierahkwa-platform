namespace Mamey.Http;

public interface IApiResponseHandler
{
    Task<ApiResponse> HandleAsync(Task<ApiResponse> request);
    Task<T> HandleAsync<T>(Task<ApiResponse<T>> request);
    
}
