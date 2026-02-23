using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
using Mamey.Blazor.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Clients;

public static class Extensions
{
    public static IServiceCollection AddIdentityBlazorClients(this IServiceCollection services)
    {
        // services.AddHttpClient<IAuthenticationApiClient<Login>>("auth-api", client =>
        // {
        //     // client.BaseAddress = new Uri("https://localhost:51816/");
        //     // client.DefaultRequestHeaders.Add("X-API-KEY", "a6b11c4e-b47e-496d-99a4-a4d634db25a1");
        //     // client.BaseAddress = new Uri("https://localhost:7295/");
        // });
        // services.AddScoped<IAuthenticationApiClient<Login>>(sp =>
        // {
        //     var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
        //     var client = clientFactory.CreateClient("auth-api");
        //     return new AuthenticationApiClient(client);
        // });
        return services;
    }
}