using System.Text;
using Elasticsearch.Net;
using Mamey.Auth.Identity;
using Mamey.Auth.Jwt;
using Mamey.Auth.Multi;
using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Discovery.Consul;
using Mamey.Docs.Swagger;
using Mamey.Http;
using Mamey.LoadBalancing.Fabio;
using Mamey.MessageBrokers;
using Mamey.MessageBrokers.Outbox;
using Mamey.MessageBrokers.Outbox.Mongo;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.Metrics.AppMetrics;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Microservice.Infrastructure.Auth;
using Mamey.Microservice.Infrastructure.Contexts;
using Mamey.Microservice.Infrastructure.Decorators;
using Mamey.Microservice.Infrastructure.Jaeger;
using Mamey.Microservice.Infrastructure.Messaging;
using Mamey.Microservice.Infrastructure.Messaging.Brokers;
using Mamey.Microservice.Infrastructure.Mongo;
using Mamey.Microservice.Infrastructure.Serialization;
using Mamey.CQRS;
using Mamey.Persistence.Redis;
using Mamey.Security;
using Mamey.Tracing.Jaeger;
using Mamey.Tracing.Jaeger.RabbitMQ;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.WebApi.Security;
using Mamey.WebApi.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using IEventProcessor = Mamey.CQRS.Events.IEventProcessor;

namespace Mamey.Microservice.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddMicroserviceSharedInfrastructure(this IMameyBuilder builder)
    {
        builder.Services.AddSerialization();
        builder.Services.AddTransient<IEventProcessor, EventProcessor>();

        // Use builder.Configuration instead of BuildServiceProvider() to avoid hangs
        var options = builder.Configuration?.GetOptions<MongoOptions>("mongo") ?? new MongoOptions();
        builder.Services.AddSingleton(options);
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
        builder.Services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        builder.Services.AddTransient<BearerTokenMiddleware>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        
        return builder
            .AddQueryHandlers()
            .AddSharedMicroserviceConfiguration()
            .AddInMemoryQueryDispatcher()
            .AddMessageOutbox(o =>
            {
                // var dbContextTypes = assemblies
                //     .SelectMany(x => x.GetTypes())
                //     .Where(x => typeof(DbContext).IsAssignableFrom(x) &&
                //                 !x.IsInterface &&
                //                 !x.IsAbstract &&
                //                 x != typeof(DbContext))
                //     .ToList();
                //
                // var efExtensionsType = typeof(Mamey.MessageBrokers.Outbox.EntityFramework.Extensions);
                //
                // // Find correct method: AddEntityFramework<T>(IMessageOutboxConfigurator)
                // var method = efExtensionsType
                //     .GetMethods(BindingFlags.Public | BindingFlags.Static)
                //     .FirstOrDefault(m =>
                //         m.Name == "AddEntityFramework" &&
                //         m.IsGenericMethodDefinition &&
                //         m.GetGenericArguments().Length == 1 &&
                //         m.GetParameters().Length == 1 &&
                //         m.GetParameters()[0].ParameterType == typeof(IMessageOutboxConfigurator));
                //
                // if (method == null)
                // {
                //     throw new InvalidOperationException("Unable to find AddEntityFramework<T>() extension method.");
                // }
                //
                // foreach (var dbContextType in dbContextTypes)
                // {
                //     var genericMethod = method.MakeGenericMethod(dbContextType);
                //     genericMethod.Invoke(null, new object[] { o });
                // }
                o.AddMongo();
                
            })
            ;
    }

    public static IMameyBuilder AddSagaInfrastructure(this IMameyBuilder builder)
    {
        

        builder
            .AddSharedMicroserviceConfiguration()
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher();
          

        return builder;
    }
    private static IMameyBuilder AddSharedMicroserviceConfiguration(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IRng, Rng>();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddTransient<IMessageBroker, MessageBroker>();
        builder.Services.AddTransient<IContextFactory, ContextFactory>();
        builder.Services.AddTransient<IContext>(ctx => ctx.GetRequiredService<IContextFactory>().Create());
        builder.Services.AddScoped<ICache, RedisCache>();
        builder.Services.AddRouting();
        builder.Services.AddSingleton<ISerializer, SystemTextJsonSerializer>();
        return builder
            .AddMultiAuth()
            // .AddMameyAuthIdentity()
            .AddMameyNet()
            .AddHttpClient()
            .AddConsul()
            .AddFabio()
            .AddRedis()
            .AddMetrics()
            .AddJaeger(openTracingBuilder: tracingBuilder =>
            {
                // Enable automatic span creation for HTTP requests
                tracingBuilder.ConfigureAspNetCore(options =>
                {
                    // Optionally exclude health checks or static files
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.Request.Path.StartsWithSegments("/health"));
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.Request.Path.StartsWithSegments("/_blazor"));
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.Request.Path.StartsWithSegments("/favicon.ico"));
                });

                // Entity Framework Core query tracing
                tracingBuilder.ConfigureEntityFrameworkCore(options =>
                {
                    // Customize operation names to show command type and verb
                    options.OperationNameResolver = data =>
                    {
                        var verb = data.Command.CommandText?.Split(' ', '\n', '\r', '\t')?[0]?.Trim().ToUpperInvariant();
                        return $"EF {data.Command.CommandType} {verb}";
                    };

                    // Filter out noisy metadata queries (e.g., migrations, schema introspection)
                    options.IgnorePatterns.Add(data =>
                        data.Command.CommandText?.Contains("__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase) == true ||
                        data.Command.CommandText?.Contains("pg_class", StringComparison.OrdinalIgnoreCase) == true ||
                        data.Command.CommandText?.Contains("information_schema", StringComparison.OrdinalIgnoreCase) == true);

                    // Optional: Set component tag used in tracing UI
                    options.ComponentName = "EFCore.Mamey";
                });
            })
            .AddJaegerDecorators()
            .AddRabbitMq()
            .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
       
            .AddWebApiSwaggerDocs() 
            .AddCertificateAuthentication()
            .AddSecurity();
    }

    public static IApplicationBuilder UseSharedInfrastructure(this IApplicationBuilder builder)
    {
        builder
            .UseRouting();
        
        return builder
            .UseMultiAuth()
            .UseMamey()
            .UseErrorHandler()
            .UseSwaggerDocs()
            .UseJaeger()
            .UseCertificateAuthentication()
            // .UseAuthentication()
            // .UseAuthorization()
            .UsePublicContracts<ContractAttribute>()
            .UseMetrics()
            // .UseMameyAuthIdentity()
            .UseAccessTokenValidator()
            ;
    }
    public static IApplicationBuilder UseSagaSharedInfrastructure(this IApplicationBuilder builder)
    {
        builder
            .UseRouting();

        return builder
            //.UseMiddleware<BearerTokenMiddleware>()
            .UseAccessTokenValidator()
            .UseMamey()
            .UseErrorHandler()
            .UseSwaggerDocs()
            .UseMetrics()
            .UseJaeger()
            .UseCertificateAuthentication()
            .UseAuthentication()
            .UseAuthorization()
            .UsePublicContracts<ContractAttribute>()
            .UseMetrics();
    }

    public static async Task<Guid> AuthenticateUsingJwtAsync(this HttpContext context)
    {
        var authentication = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        if (!authentication.Succeeded || authentication.Principal?.Identity?.Name is null)
        {
            return Guid.Empty;
        }

        return Guid.TryParse(authentication.Principal.Identity.Name, out var userId) ? userId : Guid.Empty;
    }

    internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
    {
        if (messageProperties is null)
        {
            return string.Empty;
        }

        if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
        {
            return Encoding.UTF8.GetString(spanBytes);
        }

        return string.Empty;
    }
    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    #region Messaging Extensions
    internal static CorrelationContext? GetCorrelationContext(this IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext?.Request.Headers.TryGetValue("Correlation-Context", out var json) is true)
        {
            var jsonValue = json.FirstOrDefault();
            if (string.IsNullOrEmpty(jsonValue))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<CorrelationContext>(jsonValue);
        }
        return null;
    }

    internal static IDictionary<string, object> GetHeadersToForward(this IMessageProperties messageProperties)
    {
        const string sagaHeader = "Saga";
        if (messageProperties?.Headers is null || !messageProperties.Headers.TryGetValue(sagaHeader, out var saga))
        {
            return null;
        }

        return saga is null
            ? null
            : new Dictionary<string, object>
            {
                [sagaHeader] = saga
            };
    }

    #endregion
}

