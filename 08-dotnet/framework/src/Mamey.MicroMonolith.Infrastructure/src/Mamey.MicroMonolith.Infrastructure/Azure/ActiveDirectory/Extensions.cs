//using System.Collections.Generic;
//using System.Linq;
//using System.Security;
//using Azure.Identity;
//using Mamey.MicroMonolith.Abstractions.Modules;
//using Mamey.MicroMonolith.Infrastructure.Auth;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization.Policy;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Graph;
//using Microsoft.Identity.Client;
//using Microsoft.Identity.Web;

//namespace Mamey.MicroMonolith.Infrastructure.Azure.ActiveDirectory;

//public static class Extensions
//{
//    public static IServiceCollection AddMameyActiveDirectory(this IServiceCollection services, IList<IModule> modules = null)
//    {
        
//        // GetAccountByIdAsync configuration from collection
//        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

//        var adOptions = services.GetOptions<ActiveDirectoryOptions>("azureAd");
//        services.AddSingleton(adOptions);

//        if(adOptions.Enabled)
//        {
//            if (adOptions.AuthenticationDisabled)
//            {
//                services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
//            }
//            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//            //    .AddMicrosoftIdentityWebApi(configuration.GetSection("azureAd"))
//            //        .EnableTokenAcquisitionToCallDownstreamApi()
//            //        //   .AddUseFHGUICoreServerMicrosoftGraph(configuration
//            //        //         .GetSection("downstreamApi"))
//            //          .AddDownstreamWebApi("MameyInternalAPI",
//            //                configuration.GetSection("DownstreamApi"))
//            //          .AddInMemoryTokenCaches();


//            var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
//                           Enumerable.Empty<string>();
//            services.AddAuthorization(authorization =>
//            {
//                foreach (var policy in policies)
//                {
//                    authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
//                }
//            });

//            services.AddSingleton(CreateGraphServiceClient(configuration));
//        }
//        return services;
//    }


    


//    /// <summary>
//    /// To use with Console applications.
//    /// </summary>
//    /// <param name="configuration"></param>
//    /// <param name="username"></param>
//    /// <param name="password"></param>
//    /// <returns></returns>
//    private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot configuration, string username, SecureString password)
//    {
//        var authenticationProvider = CreateAuthorizationProvider(configuration, username, password);
//        var graphClient = new GraphServiceClient(authenticationProvider);
//        return graphClient;
//    }

//    private static IAuthenticationProvider CreateAuthorizationProvider(IConfiguration configuration, string username, SecureString password)
//    {
//        var clientId = configuration["azureAd:clientId"];
//        var authority = $"https://login.microsoft.com/{configuration["azureId:tenantId"]}/v2.0";

//        List<string> scopes = new List<string>();
//        scopes.Add("User.Read");
//        scopes.Add("User.ReadAll");
//        scopes.Add("API.Access");

//        var cca = PublicClientApplicationBuilder.Create(clientId)
//            .WithAuthority(authority)
//            .Build();
        
//        return MsalAuthenticationProvider.GetInstance(cca, scopes.ToArray(), username, password);
//    }
//    private static GraphServiceClient CreateGraphServiceClient(IConfiguration configuration)
//    {
//        //List<string> scopes = new List<string>();
//        //scopes.Add("User.Read");
//        //scopes.Add("User.ReadAll");
//        //scopes.Add("API.Access");
//        var scopes = new[] { "https://graph.microsoft.com/.default" };

//        var tenantId = configuration["azureAd:tenantId"];
//        var clientId = configuration["azureAd:clientId"];
//        var clientSecret = configuration["azureAd:clientSecret"];

//        var options = new TokenCredentialOptions
//        {
//            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
//        };


//        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
//        var clientSecretCredential = new ClientSecretCredential(
//            tenantId, clientId, clientSecret, options);

//        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
//        return graphClient;
//    }
//}