namespace Mamey.Graph.Msal;

public static class Extensions
{
    //public static IMameyBuilder AddMsal(this IMameyBuilder builder, List<string>? scopes = null)
    //{
    //    var graphOptions = builder.Services.GetOptions<GraphOptions>("azureAd");
    //    if (scopes == null || !scopes.Any())
    //    {
    //        scopes = graphOptions.Scopes;
    //        if (scopes == null || !scopes.Any())
    //        {
    //            throw new MameyException("Missing MsalCofiguration");
    //        }
    //    }
    //    builder.Services.Configure<RemoteAuthenticationOptions<MsalProviderOptions>>(
    //        options =>
    //        {
    //            scopes?.ForEach((scope) =>
    //            {
    //                options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
    //            });

    //        });
    //    return builder;
    //}
}

