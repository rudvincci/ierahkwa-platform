using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mamey.Auth.Identity;
using Mamey.Barcode;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.ISO3166;
using Mamey.MicroMonolith.Abstractions.Dispatchers;
using Mamey.MicroMonolith.Abstractions.Storage;
using Mamey.MicroMonolith.Abstractions.Time;
using Mamey.MicroMonolith.Infrastructure.Api;
using Mamey.MicroMonolith.Infrastructure.Contexts;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Dispatchers;
using Mamey.MicroMonolith.Infrastructure.Exceptions;
using Mamey.MicroMonolith.Infrastructure.Kernel;
using Mamey.MicroMonolith.Infrastructure.Logging;
using Mamey.MicroMonolith.Infrastructure.Messaging;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.MicroMonolith.Infrastructure.Security;
using Mamey.MicroMonolith.Infrastructure.Serialization;
using Mamey.MicroMonolith.Infrastructure.Storage;
using Mamey.MicroMonolith.Infrastructure.Time;
using Mamey.Modules;
using Mamey.Persistence.Redis;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Mamey.Services;
using Mamey.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Mamey.MicroMonolith.Infrastructure;

public static class Extensions
{
    private const string CorrelationIdKey = "correlation-id";
    private static IConfiguration configuration;
        
    public static IServiceCollection AddInitializer<T>(this IServiceCollection services) where T : class, IInitializer
        => services.AddTransient<IInitializer, T>();

    public static IMameyBuilder AddModularInfrastructure(this IMameyBuilder builder, IList<Assembly> assemblies,
        IList<IModule> modules)
    {
        builder.Services.ConfigureInfrastructure(assemblies, modules);
        return builder
            .ConfigureServices(assemblies, modules)
            .AddRedis()
            .AddBarcode();
    }
    
    private static IServiceCollection ConfigureInfrastructure(this IServiceCollection services,
        IList<Assembly> assemblies, IList<IModule> modules) 
    {
        var appOptions = services.GetOptions<AppOptions>("app");
        services.AddSingleton(appOptions);
        
        var disabledModules = new List<string>();
        using (var serviceProvider = services.BuildServiceProvider())
        {
            configuration = serviceProvider.GetRequiredService<IConfiguration>();
            foreach (var (key, value) in configuration.AsEnumerable())
            {
                if (!key.Contains(":module:enabled"))
                {
                    continue;
                }

                if (!bool.Parse(value))
                {
                    disabledModules.Add(key.Split(":")[0]);
                }
            }
        }

        services.AddCorsPolicy();
        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.CustomSchemaIds(x => x.FullName);
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{appOptions?.Name} Modular API v{appOptions?.Version}",
                Version = $"v{appOptions?.Version}",
                Description = "Banking System API"
            });
        });

        services.AddControllers(options =>
            {
                // options.Filters.Add<ExceptionHandlingFilter>();
                // options.Filters.Add<ApiKeyAuthorizationFilter>();
            })
            .ConfigureApplicationPartManager(manager =>
            {
                var removedParts = new List<ApplicationPart>();
                foreach (var disabledModule in disabledModules)
                {
                    var parts = manager.ApplicationParts.Where(x => x.Name.Contains(disabledModule,
                        StringComparison.InvariantCultureIgnoreCase));
                    removedParts.AddRange(parts);
                }

                foreach (var part in removedParts)
                {
                    manager.ApplicationParts.Remove(part);
                }
                    
                manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
            });
        
        
        
        return services;
    }

    public static async Task<IApplicationBuilder> UseModularInfrastructure(this IApplicationBuilder app, 
        Dictionary<Type, Dictionary<string, long>>? rolePermissions = null)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseCors("cors");
        app.UseCorrelationId();
        app.UseErrorHandling();
        app.UseSwagger();
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = "docs";
            reDoc.SpecUrl("/swagger/v1/swagger.json");
            reDoc.DocumentTitle = "Modular API";
        });
        // app.UseAuth();
        app.UseContext();
        app.UseLogging();
        app.UseRouting();
       // app.UseMameyAuthIdentity();
        // app.UseAntiforgery();
        // app.UseAuthentication();
        // app.UseAuthorization();
        app.UseISO3166();
        return app;
    }

    private static IMameyBuilder ConfigureServices(this IMameyBuilder builder, IList<Assembly> assemblies,
        IList<IModule> modules)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        
        builder.Services.AddSingleton<IRequestStorage, RequestStorage>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient();
        builder.Services.AddModuleInfo(modules);
        builder.Services.AddModuleRequests(assemblies);
        // builder.Services.AddAuth(modules);
        builder.Services.AddErrorHandling();
        builder.Services.AddContext();
        
        builder.Services.AddMessaging();
        builder.Services.AddSecurity();
        builder.Services.AddSingleton<IClock, UtcClock>();
        builder
            .AddScopedCommandHandlers(assemblies)
            .AddScopedEventHandlers(assemblies)
            .AddScopedQueryHandlers(assemblies)
            .AddInMemoryCommandDispatcher()
            .AddInMemoryEventDispatcher()
            .AddInMemoryQueryDispatcher()
            ;
        builder.Services.AddSingleton<IDispatcher, InMemoryDispatcher>();
        builder.Services.AddLoggingDecorators();
        
        // databse configuration
        builder.Services.AddPostgres();
        builder.Services.AddMongo();
        builder.Services.AddOutbox();
        builder.AddMameyPersistence();
        builder.Services.AddTransactionalDecorators();
        
        // Register completion service for coordinating hosted service execution order
        builder.Services.AddSingleton<DatabaseInitializationCompletionService>();
        
        builder.Services.AddHostedService<DbContextAppInitializer>();
        builder.Services.AddHostedService<AppInitializer>();
        builder.Services.AddContracts();
        
        builder.Services.AddSignalR();
        builder.Services.AddISO3166();
        
        builder.Services.AddScoped<ICache, RedisCache>();
        builder.Services.AddDomainEvents(assemblies);
        // builder.Services.AddEvents(assemblies);
        return builder;
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

    public static string GetModuleName(this object value)
        => value?.GetType().GetModuleName() ?? string.Empty;

    public static string GetModuleName(this Type type, string namespacePart = "Modules", int splitIndex = 2)
    {
        if (type?.Namespace is null)
        {
            return string.Empty;
        }

        return type.Namespace.Contains(namespacePart)
            ? type.Namespace.Split(".")[splitIndex].ToLowerInvariant()
            : string.Empty;
    }
        
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.Use((ctx, next) =>
        {
            ctx.Items.Add(CorrelationIdKey, Guid.NewGuid());
            return next();
        });
        
    public static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var id) ? (Guid) id : null;
}