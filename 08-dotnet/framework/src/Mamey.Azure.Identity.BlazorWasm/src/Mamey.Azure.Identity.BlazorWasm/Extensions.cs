using Mamey.Azure.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Mamey.Azure.Identity.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddAzureIdentityBlazorWasm(this IMameyBuilder builder, string scope)
    {
        var graphOptions = builder.Services.GetOptions<AzureBlazorWasmOptions>(AzureOptions.APPSETTINGS_SECTION);

        if(string.IsNullOrEmpty(graphOptions.Scopes))
        {
            graphOptions.Scopes = scope;
        }
        builder.Services.AddSingleton(graphOptions);

        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind(AzureOptions.APPSETTINGS_SECTION, options.ProviderOptions.Authentication);
            //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://INKGWebExt.onmicrosoft.com/Ink.Web.LandingServer/Api.ReadWrite");
            //options.ProviderOptions.DefaultAccessTokenScopes.Add("api://ba7074ea-e24e-479c-9532-a17be7a076d1/API.Access");
            options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
        });



        //builder.Services.AddMsalAuthentication(options =>
        //{
        //    var configuration = builder.Services
        //        .BuildServiceProvider()
        //        .GetRequiredService<IConfiguration>();

        //    configuration.Bind(AzureOptions.APPSETTINGS_SECTION, options.ProviderOptions.Authentication);
        //    options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);

        //});


        //builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>(f);
        // builder.Services.AddScoped<TokenAuthenticationStateProvider>();
        // builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
        //builder.Services.AddScoped<IBlazorAuthenticationService, BlazorAuthenticationService>();
        return builder;
    }
}

