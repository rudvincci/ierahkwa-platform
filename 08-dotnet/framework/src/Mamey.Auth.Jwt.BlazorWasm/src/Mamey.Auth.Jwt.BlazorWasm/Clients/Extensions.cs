using Mamey.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Auth.Jwt.BlazorWasm.Clients;

public static class Extensions
{
    public static IMameyBuilder AddJwtAuthApiClients(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IHttpClient, ApiGatewayClient>();
        return builder;
    }
}