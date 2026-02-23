// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Graph;
// using Microsoft.Identity.Client;
// using StackExchange.Redis;
//
//
// namespace Mamey.Auth.Azure.B2B;
//
// public static class Extensions
// {
//     private const string SectionName = "azureB2B";
//     public static IMameyBuilder AddB2BAuth(this IMameyBuilder builder, string? sectionName = null)
//     {
//         if (string.IsNullOrWhiteSpace(sectionName))
//         {
//             sectionName = SectionName;
//         }
//         var b2bOptions = builder.GetOptions<AzureB2BOptions>(sectionName);
//         var redisOptions = builder.GetOptions<RedisOptions>("redis");
//         builder.Services.AddSingleton(b2bOptions);
//
//         // var authOptions = new AuthenticationOptions
//         // {
//         //     ClientId = configuration["AzureAdB2B:ClientId"],
//         //     ClientSecret = configuration["AzureAdB2B:ClientSecret"],
//         //     Authority = configuration["AzureAdB2B:Authority"]
//         // };
//
//         // builder.Services.AddSingleton(authOptions);
//         builder.Services.AddSingleton<IConfidentialClientApplication>(sp =>
//         {
//             return ConfidentialClientApplicationBuilder.Create(b2bOptions.ClientId)
//                 .WithClientSecret(b2bOptions.ClientSecret)
//                 .WithAuthority(new Uri(b2bOptions.Authority))
//                 .Build();
//         });
//
//         // builder.Services.AddSingleton<GraphServiceClient>(sp =>
//         // {
//         //     var authProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
//         //     {
//         //         var token = await sp.GetRequiredService<IConfidentialClientApplication>()
//         //             .AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
//         //             .ExecuteAsync();
//
//         //         requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
//         //     });
//
//         //     return new GraphServiceClient(authProvider);
//         // });
//
//         // Add Redis caching
//         var redisConnection = ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
//         builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
//         builder.Services.AddSingleton<IRedisTokenCache, RedisTokenCache>();
//
//         builder.Services.AddScoped<IB2BAuthenticationService, B2BAuthenticationService>();
//
//
//
//         return builder;
//     }
//     public static WebApplication UseB2BAuth(this WebApplication app)
//     {
//         app.UseAuthorization();
//         return app;
//     }
// }
