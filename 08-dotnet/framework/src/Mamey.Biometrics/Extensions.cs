using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Minio;
using Mamey.Biometrics.Engine;
using Mamey.Biometrics.Services;
using Mamey.Biometrics.Storage.MongoDB;
using Mamey.Biometrics.Storage.MinIO;
using Polly;
using Polly.Extensions.Http;

namespace Mamey.Biometrics;

/// <summary>
/// Extension methods for registering biometric services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds biometric services to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration section for biometric options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyBiometrics(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
            
        // Configure options
        services.Configure<BiometricsOptions>(configuration.GetSection(BiometricsOptions.SectionName));
        
        // Register HTTP client with Polly policies
        services.AddHttpClient<IBiometricEngineClient, BiometricEngineClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BiometricsOptions>>().Value;
            client.BaseAddress = new Uri(options.BiometricsEngineBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register storage services
        services.AddSingleton<IBiometricTemplateRepository, BiometricTemplateRepository>();
        services.AddSingleton<IBiometricImageStore, BiometricImageStore>();
        
        // Register business services
        services.AddScoped<IBiometricService, BiometricService>();
        
        // Register memory cache
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds biometric services to the service collection with custom options.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure biometric options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyBiometrics(
        this IServiceCollection services, 
        Action<BiometricsOptions> configureOptions)
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));
            
        // Configure options
        services.Configure(configureOptions);
        
        // Register HTTP client with Polly policies
        services.AddHttpClient<IBiometricEngineClient, BiometricEngineClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BiometricsOptions>>().Value;
            client.BaseAddress = new Uri(options.BiometricsEngineBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register storage services
        services.AddSingleton<IBiometricTemplateRepository, BiometricTemplateRepository>();
        services.AddSingleton<IBiometricImageStore, BiometricImageStore>();
        
        // Register business services
        services.AddScoped<IBiometricService, BiometricService>();
        
        // Register memory cache
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds MongoDB configuration for biometric templates.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration section for MongoDB</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyBiometricsMongoDB(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Configure MongoDB options
        services.Configure<MongoDBOptions>(configuration.GetSection("MongoDB"));
        
        // Register MongoDB client and database
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDBOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });
        
        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var options = serviceProvider.GetRequiredService<IOptions<MongoDBOptions>>().Value;
            return client.GetDatabase(options.DatabaseName);
        });
        
        return services;
    }

    /// <summary>
    /// Adds MinIO configuration for biometric images.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration section for MinIO</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyBiometricsMinIO(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Configure MinIO options
        services.Configure<MinIOOptions>(configuration.GetSection("MinIO"));
        
        // Register MinIO client
        services.AddSingleton<IMinioClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MinIOOptions>>().Value;
            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSSL)
                .Build();
        });
        
        return services;
    }

    /// <summary>
    /// Creates retry policy for HTTP calls.
    /// </summary>
    /// <returns>Retry policy</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
                });
    }

    /// <summary>
    /// Creates circuit breaker policy for HTTP calls.
    /// </summary>
    /// <returns>Circuit breaker policy</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {duration}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });
    }
}
