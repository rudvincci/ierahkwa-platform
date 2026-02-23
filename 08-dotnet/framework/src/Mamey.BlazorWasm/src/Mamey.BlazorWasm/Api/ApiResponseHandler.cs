//using System;
//using System.Net;
//using System.Reactive.Linq;
//using Mamey.Http;
//using Microsoft.Extensions.Logging;

//namespace Mamey.BlazorWasm.Api;

//public class ApiResponseHandler : IApiResponseHandler
//{
//    private readonly ILogger<ApiResponseHandler> _logger;  
//    private const int ModalDurationMilliseconds = 1000;

//    public ApiResponseHandler(ILogger<ApiResponseHandler> logger)
//    {
//        _logger = logger;
//    }

//    public async Task<ApiResponse> HandleAsync(Task<ApiResponse> request)
//    {
//        var response = await request;
//        if (response.Succeeded)
//        {
//            return response;
//        }

//        await HandleErrorAsync(response);
//        return default;
//    }

//    public async Task<T> HandleAsync<T>(Task<ApiResponse<T>> request)
//    {
//        var response = await request;
//        if (response.Succeeded)
//        {
//            return response.Value;
//        }

//        await HandleErrorAsync(response);
//        return default;
//    }
//    public async Task<IObservable<T>> HandleAsObservableAsync<T>(Task<ApiResponse<T>> request)
//    {
//        var response = await request;
//        if (response.Succeeded)
//        {
//            return Observable.Start(() => response.Value);
//        }

//        await HandleErrorAsync(response);
//        return default;
//    }

//    private async Task HandleErrorAsync(ApiResponse response)
//        => await Task.FromResult(() =>
//        {
//            if (response?.HttpResponse is null)
//            {
//                return;
//            }

//            if (response.HttpResponse.StatusCode == HttpStatusCode.Unauthorized)
//            {
//                return;
//            }

//            if (response.Error is { })
//            {
//            }

//        });
//}
