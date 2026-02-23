using Mamey.Blazor.Abstractions.Api;
using Mamey.BlazorWasm.Api;
using Mamey.BlazorWasm.Components.Grid;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddBlazorWasm(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<AppState>();
        builder.Services.AddScoped(typeof(IReactiveService<,>), typeof(ReactiveService<,>));
        builder.Services.AddScoped<IObservableApiResponseHandler, ObservableApiResponseHandler>();
        // builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

        
        
        //services.AddTokenAuthenticationStateProvider();
   
        return builder;
    }
}

