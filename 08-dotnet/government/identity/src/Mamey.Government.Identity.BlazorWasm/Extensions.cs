using System;
using Mamey.Bank.Accounts.BlazorWasm.Services;
using Mamey.Government.Identity.BlazorWasm.Clients;
using Mamey.Government.Identity.BlazorWasm.Providers;
using Mamey.Government.Identity.BlazorWasm.Storage;
using Mamey.Government.Identity.BlazorWasm.ViewModels.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.BlazorWasm;

public static class Extensions
{
    private const string RegistryName = "identity-blazor-wasm";
    public static IMameyBuilder AddIdentityBlazorWasm(this IMameyBuilder builder, string? apiUrl = null)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.AddIdentityClient(apiUrl);
        
        // Register ITokenStore and ProtectedLocalStorage first (dependencies)
        builder.Services.AddProtectedLocalStorage();
        
        // Register ApiAuthenticationStateProvider (depends on ITokenStore)
        builder.Services.AddScoped<ApiAuthenticationStateProvider>();
        
        // Register AuthenticationStateProvider to use ApiAuthenticationStateProvider
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<ApiAuthenticationStateProvider>());
        
        builder.Services.AddAuthorizationCore();
        builder.Services.AddServices();
        builder.Services.AddScoped<LoginViewModel>();
        return builder;
    }


    public static IApplicationBuilder UseIdentityBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}

