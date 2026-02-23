using System.Net;
using System.Reactive.Linq;
using Mamey.Blazor.Abstractions.Api;
using Mamey.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.BlazorWasm.Api;
public class ObservableApiResponseHandler : ApiResponseHandler, IObservableApiResponseHandler
{
    private readonly ILogger<ObservableApiResponseHandler> _logger;
    private const int ModalDurationMilliseconds = 1000;

    public ObservableApiResponseHandler(ILogger<ObservableApiResponseHandler> logger)
        : base(logger)
    {
        _logger = logger;
    }

    public async Task<IObservable<T>> HandleAsObservableAsync<T>(Task<ApiResponse<T>> request)
    {
        var response = await request;
        if (response.Succeeded)
        {
            return Observable.Start(() => response.Value);
        }

        await HandleErrorAsync(response);
        return default;
    }

    private async Task HandleErrorAsync(ApiResponse response)
        => await Task.FromResult(() =>
        {
            if (response?.HttpResponse is null)
            {
                return;
            }

            if (response.HttpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            if (response.Error is { })
            {
            }

        });
}

