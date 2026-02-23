using Mamey.Auth.Jwt.BlazorWasm.Clients;
using Mamey.Auth.Jwt.BlazorWasm.Services;

namespace Mamey.Auth.Jwt.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddJwtAuthBlazorWasm(this IMameyBuilder builder)
    {
       
        return builder
            .AddJwtAuthApiClients()
            .AddJwtAuthBlazorWasmServices();
    }
}

