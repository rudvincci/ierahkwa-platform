using App.Metrics.AspNetCore;
using INKG.CitizenPortal.ApiGataway.Infrastructure;
using Mamey;
using Mamey.Auth.Jwt;
using Mamey.Logging;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.Metrics.AppMetrics;
using Mamey.Secrets.Vault;
using Mamey.Security;
using Mamey.Tracing.Jaeger;
using Mamey.Types;
using Mamey.WebApi;
using Microsoft.AspNetCore.Authentication;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

namespace INKG.CitizenPortal.ApiGataway;

public class Program
{
    public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();


    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile("ocelot.json", false, true)
                .AddEnvironmentVariables();
        })
        .ConfigureWebHostDefaults(webhostBuilder =>
        {
            var config = webhostBuilder.ConfigureAppConfiguration(c=>
                c.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args))
                ;
            webhostBuilder.ConfigureServices(services =>
            {

                services.AddHttpClient();
                services.AddSingleton<IPayloadBuilder, PayloadBuilder>();
                services.AddSingleton<ICorrelationContextBuilder, CorrelationContextBuilder>();
                services.AddSingleton<IAnonymousRouteValidator, AnonymousRouteValidator>();
                services.AddTransient<AsyncRoutesMiddleware>();
                services.AddTransient<ResourceIdGeneratorMiddleware>();

                var config = services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>();

                services
                    .AddOcelot(config)
                        .AddPolly()
                        .AddDelegatingHandler<CorrelationContextHandler>(true);
                services
                        .AddMamey(configuration: config)
                        .AddWebApi()
                        .AddMetrics()
                        .AddErrorHandler<ExceptionToResponseMapper>()
                        .AddJaeger()
                        .AddJwt()
                        .AddRabbitMq()
                        .AddWebApi()
                        .AddSecurity()
                        .Build()
                        ;

                using var provider = services.BuildServiceProvider();
                var configuration = provider.GetService<IConfiguration>();
                services.Configure<AsyncRoutesOptions>(configuration.GetSection("AsyncRoutes"));
                services.Configure<AnonymousRoutesOptions>(configuration.GetSection("AnonymousRoutes"));
     
            })
            .Configure(app =>
            {
                app.UseMamey();
                app.UseErrorHandler();
                // app.UseAccessTokenValidator();
                app.UseRabbitMq();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.Get("/", (ctx) =>
                    {
                        var appOptions = ctx.RequestServices.GetRequiredService<AppOptions>();
                        return ctx.Response.WriteAsync(appOptions.Name);
                    });
                });
                app.UseMiddleware<AsyncRoutesMiddleware>();
                app.UseMiddleware<ResourceIdGeneratorMiddleware>();
                app.UseAuthentication();
                app.UseOcelot(GetOcelotConfiguration()).GetAwaiter().GetResult();
            });
        })
        .UseLogging()
        .UseVault()
        .UseMetrics();

    private static OcelotPipelineConfiguration GetOcelotConfiguration()
        => new OcelotPipelineConfiguration
        {
            AuthenticationMiddleware = async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    await next.Invoke();
                    return;
                }

                if (context.RequestServices.GetRequiredService<IAnonymousRouteValidator>()
                    .HasAccess(context.Request.Path))
                {
                    await next.Invoke();
                    return;
                }

                var authenticateResult = await context.AuthenticateAsync();
                if (authenticateResult.Succeeded)
                {
                    context.User = authenticateResult.Principal;
                    await next.Invoke();
                    return;
                }
                await context.Response.Unauthorized();
                
                //context.ResponseErrors.Add(new UnauthenticatedError("Unauthenticated"));
            }
        };
}

