// MyApp.Client/Services/ReactiveAuthenticationService.cs

using Mamey.BlazorWasm.Http;
using Microsoft.Extensions.DependencyInjection;
using Mamey.BlazorWasm.Api;

namespace Mamey.Auth.Jwt.BlazorWasm.Services;

public static class Extensions
{
    public static IMameyBuilder AddJwtAuthBlazorWasmServices(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
        builder.Services.AddScoped<IAuthenticationService, ReactiveAuthenticationService>();

        return builder;
    }
}
