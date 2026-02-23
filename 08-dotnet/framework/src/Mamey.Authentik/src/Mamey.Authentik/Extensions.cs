using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Handlers;
using Mamey.Authentik.Policies;
using Mamey.Authentik.Services;
using Polly;

namespace Mamey.Authentik;

/// <summary>
/// Extension methods for registering Authentik services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Authentik services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure Authentik options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthentik(
        this IServiceCollection services,
        Action<AuthentikOptions> configure)
    {
        services.Configure(configure);
        var options = new AuthentikOptions();
        configure(options);
        options.Validate();

        return services.AddAuthentikCore();
    }

    /// <summary>
    /// Adds Authentik services to the service collection using configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthentik(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthentikOptions>(configuration);
        var options = configuration.Get<AuthentikOptions>() ?? new AuthentikOptions();
        options.Validate();

        return services.AddAuthentikCore();
    }

    private static IServiceCollection AddAuthentikCore(this IServiceCollection services)
    {
        // Register caching
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.TryAddSingleton<IAuthentikCache, InMemoryAuthentikCache>();

        // Register handlers
        services.AddTransient<AuthentikAuthenticationHandler>();
        services.AddTransient<AuthentikLoggingHandler>();
        services.AddTransient<AuthentikErrorHandler>();

        // Register HTTP client factory with handlers for services to use
        services.AddHttpClient("Authentik", (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = options.Timeout;
        })
        .AddHttpMessageHandler<AuthentikAuthenticationHandler>()
        .AddHttpMessageHandler<AuthentikLoggingHandler>()
        .AddHttpMessageHandler<AuthentikErrorHandler>()
        .AddPolicyHandler((sp, request) =>
        {
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>().Value;
            return AuthentikRetryPolicy.CreatePolicy(options);
        })
        .AddPolicyHandler((sp, request) => AuthentikCircuitBreakerPolicy.CreatePolicy());

        // Register services (with optional cache injection)
        services.AddScoped<IAuthentikAdminService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikAdminService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikAdminService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikCoreService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikCoreService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikCoreService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikOAuth2Service>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikOAuth2Service>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikOAuth2Service(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikFlowsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikFlowsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikFlowsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikPoliciesService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikPoliciesService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikPoliciesService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikProvidersService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikProvidersService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikProvidersService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikStagesService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikStagesService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikStagesService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikSourcesService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikSourcesService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikSourcesService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikEventsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikEventsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikEventsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikAuthenticatorsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikAuthenticatorsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikAuthenticatorsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikCryptoService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikCryptoService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikCryptoService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikPropertyMappingsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikPropertyMappingsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikPropertyMappingsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikRacService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikRacService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikRacService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikRbacService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikRbacService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikRbacService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikTenantsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikTenantsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikTenantsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikTasksService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikTasksService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikTasksService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikOutpostsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikOutpostsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikOutpostsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikEndpointsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikEndpointsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikEndpointsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikEnterpriseService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikEnterpriseService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikEnterpriseService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikManagedService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikManagedService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikManagedService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikReportsService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikReportsService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikReportsService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikRootService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikRootService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikRootService(httpClientFactory, options, logger, cache);
        });
        
        services.AddScoped<IAuthentikSsfService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<AuthentikOptions>>();
            var logger = sp.GetRequiredService<ILogger<AuthentikSsfService>>();
            var cache = sp.GetService<IAuthentikCache>();
            return new AuthentikSsfService(httpClientFactory, options, logger, cache);
        });

        // Register main client
        services.AddScoped<IAuthentikClient>(sp =>
        {
            var admin = sp.GetRequiredService<IAuthentikAdminService>();
            var core = sp.GetRequiredService<IAuthentikCoreService>();
            var oauth2 = sp.GetRequiredService<IAuthentikOAuth2Service>();
            var flows = sp.GetRequiredService<IAuthentikFlowsService>();
            var policies = sp.GetRequiredService<IAuthentikPoliciesService>();
            var providers = sp.GetRequiredService<IAuthentikProvidersService>();
            var stages = sp.GetRequiredService<IAuthentikStagesService>();
            var sources = sp.GetRequiredService<IAuthentikSourcesService>();
            var events = sp.GetRequiredService<IAuthentikEventsService>();
            var authenticators = sp.GetRequiredService<IAuthentikAuthenticatorsService>();
            var crypto = sp.GetRequiredService<IAuthentikCryptoService>();
            var propertyMappings = sp.GetRequiredService<IAuthentikPropertyMappingsService>();
            var rac = sp.GetRequiredService<IAuthentikRacService>();
            var rbac = sp.GetRequiredService<IAuthentikRbacService>();
            var tenants = sp.GetRequiredService<IAuthentikTenantsService>();
            var tasks = sp.GetRequiredService<IAuthentikTasksService>();
            var outposts = sp.GetRequiredService<IAuthentikOutpostsService>();
            var endpoints = sp.GetRequiredService<IAuthentikEndpointsService>();
            var enterprise = sp.GetRequiredService<IAuthentikEnterpriseService>();
            var managed = sp.GetRequiredService<IAuthentikManagedService>();
            var reports = sp.GetRequiredService<IAuthentikReportsService>();
            var root = sp.GetRequiredService<IAuthentikRootService>();
            var ssf = sp.GetRequiredService<IAuthentikSsfService>();

            return new AuthentikClient(admin, core, oauth2, flows, policies, providers, stages, sources, events,
                authenticators, crypto, propertyMappings, rac, rbac, tenants, tasks, outposts,
                endpoints, enterprise, managed, reports, root, ssf);
        });

        return services;
    }

    /// <summary>
    /// Adds distributed cache support for Authentik (e.g., Redis).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthentikDistributedCache(this IServiceCollection services)
    {
        services.RemoveAll<IAuthentikCache>();
        services.AddSingleton<IAuthentikCache, DistributedAuthentikCache>();
        return services;
    }
}
