using Mamey;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Domain.Blazor;

public static class Extensions
{
    private const string RegistryName = "domain-blazor";

    public static IMameyBuilder AddDomainBlazor(this IMameyBuilder builder)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        // Register all BlazorWasm micro-frontend services
        // Example:
        // builder
        //     .AddService1BlazorWasm()
        //     .AddService2BlazorWasm()
        //     .AddService3BlazorWasm();

        return builder;
    }

    public static async Task<WebApplication> UseDomainBlazorAsync(this WebApplication app)
    {
        // Configure any middleware or routing specific to this RCL
        // Example:
        // await app.UseService1BlazorWasmAsync();
        // await app.UseService2BlazorWasmAsync();

        return app;
    }
}











