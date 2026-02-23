using Mamey.Auth.Decentralized.Crypto;
using Mamey.Auth.Decentralized.Options;
using Mamey.Auth.Decentralized.Resolution;
using Mamey.Auth.Decentralized.Methods;
using Mamey.Auth.Decentralized.Methods.DidWeb;
using Mamey.Auth.Decentralized.Methods.DidKey;
using Mamey.Auth.Decentralized.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Decentralized;

/// <summary>
/// Extension methods for configuring decentralized authentication
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds decentralized authentication services to the Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="configuration">The configuration section</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddDecentralized(this IMameyBuilder builder, IConfigurationSection? configuration = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        // Register options
        builder.Services.Configure<DecentralizedOptions>(options =>
        {
            if (configuration != null)
            {
                configuration.Bind(options);
            }
        });
        
        // Register core services
        builder.Services.AddSingleton<IKeyGenerator, KeyGenerator>();
        builder.Services.AddSingleton<IDidMethodRegistry, DidMethodRegistry>();
        builder.Services.AddSingleton<IDidResolver, DidResolver>();
        builder.Services.AddSingleton<IDecentralizedHandler, DecentralizedHandler>();
        
        // Register DID methods
        builder.Services.AddSingleton<IDidMethod, DidWebMethod>();
        builder.Services.AddSingleton<IDidMethod, DidKeyMethod>();
        
        // Register HTTP client for DID Web
        builder.Services.AddHttpClient<DidWebMethod>();
        
        // Register options for DID methods
        builder.Services.Configure<DidWebOptions>(options =>
        {
            configuration?.GetSection("didWeb").Bind(options);
        });
        
        builder.Services.Configure<DidKeyOptions>(options =>
        {
            configuration?.GetSection("didKey").Bind(options);
        });
        
        return builder;
    }
    
    /// <summary>
    /// Adds decentralized authentication services with custom options
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddDecentralized(this IMameyBuilder builder, Action<DecentralizedOptions> configureOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));
        
        // Register options
        builder.Services.Configure(configureOptions);
        
        // Register core services
        builder.Services.AddSingleton<IKeyGenerator, KeyGenerator>();
        builder.Services.AddSingleton<IDidMethodRegistry, DidMethodRegistry>();
        builder.Services.AddSingleton<IDidResolver, DidResolver>();
        builder.Services.AddSingleton<IDecentralizedHandler, DecentralizedHandler>();
        
        // Register DID methods
        builder.Services.AddSingleton<IDidMethod, DidWebMethod>();
        builder.Services.AddSingleton<IDidMethod, DidKeyMethod>();
        
        // Register HTTP client for DID Web
        builder.Services.AddHttpClient<DidWebMethod>();
        
        return builder;
    }
    
    /// <summary>
    /// Adds all DID method resolvers
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddAllDidResolvers(this IMameyBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        // Register all available DID methods
        builder.Services.AddSingleton<IDidMethod, DidWebMethod>();
        builder.Services.AddSingleton<IDidMethod, DidKeyMethod>();
        
        return builder;
    }
    
    /// <summary>
    /// Adds a custom DID method
    /// </summary>
    /// <typeparam name="T">The DID method type</typeparam>
    /// <param name="builder">The Mamey builder</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddDidMethod<T>(this IMameyBuilder builder) where T : class, IDidMethod
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        builder.Services.AddSingleton<IDidMethod, T>();
        return builder;
    }
    
    /// <summary>
    /// Adds a custom DID method with options
    /// </summary>
    /// <typeparam name="T">The DID method type</typeparam>
    /// <typeparam name="TOptions">The options type</typeparam>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddDidMethod<T, TOptions>(this IMameyBuilder builder, Action<TOptions> configureOptions) 
        where T : class, IDidMethod 
        where TOptions : class, new()
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));
        
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<IDidMethod, T>();
        
        return builder;
    }
    
    /// <summary>
    /// Adds a custom cryptographic provider
    /// </summary>
    /// <typeparam name="T">The provider type</typeparam>
    /// <param name="builder">The Mamey builder</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddCryptoProvider<T>(this IMameyBuilder builder) where T : class, ICryptoProvider
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        builder.Services.AddSingleton<ICryptoProvider, T>();
        return builder;
    }
    
    /// <summary>
    /// Adds a custom cryptographic provider with options
    /// </summary>
    /// <typeparam name="T">The provider type</typeparam>
    /// <typeparam name="TOptions">The options type</typeparam>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddCryptoProvider<T, TOptions>(this IMameyBuilder builder, Action<TOptions> configureOptions) 
        where T : class, ICryptoProvider 
        where TOptions : class, new()
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));
        
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<ICryptoProvider, T>();
        
        return builder;
    }
    
    /// <summary>
    /// Adds decentralized authentication to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration section</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDecentralized(this IServiceCollection services, IConfigurationSection? configuration = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        
        // Register options
        services.Configure<DecentralizedOptions>(options =>
        {
            if (configuration != null)
            {
                configuration.Bind(options);
            }
        });
        
        // Register core services
        services.AddSingleton<IKeyGenerator, KeyGenerator>();
        services.AddSingleton<IDidMethodRegistry, DidMethodRegistry>();
        services.AddSingleton<IDidResolver, DidResolver>();
        services.AddSingleton<IDecentralizedHandler, DecentralizedHandler>();
        
        // Register DID methods
        services.AddSingleton<IDidMethod, DidWebMethod>();
        services.AddSingleton<IDidMethod, DidKeyMethod>();
        
        // Register HTTP client for DID Web
        services.AddHttpClient<DidWebMethod>();
        
        return services;
    }
    
    /// <summary>
    /// Adds decentralized authentication with custom options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDecentralized(this IServiceCollection services, Action<DecentralizedOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));
        
        // Register options
        services.Configure(configureOptions);
        
        // Register core services
        services.AddSingleton<IKeyGenerator, KeyGenerator>();
        services.AddSingleton<IDidMethodRegistry, DidMethodRegistry>();
        services.AddSingleton<IDidResolver, DidResolver>();
        services.AddSingleton<IDecentralizedHandler, DecentralizedHandler>();
        
        // Register DID methods
        services.AddSingleton<IDidMethod, DidWebMethod>();
        services.AddSingleton<IDidMethod, DidKeyMethod>();
        
        // Register HTTP client for DID Web
        services.AddHttpClient<DidWebMethod>();
        
        return services;
    }
}
