using Mamey.Http;

namespace Mamey.Blazor.Abstractions.Api;

public interface IObservableApiResponseHandler : IApiResponseHandler
{
    Task<IObservable<T>> HandleAsObservableAsync<T>(Task<ApiResponse<T>> request);
}
