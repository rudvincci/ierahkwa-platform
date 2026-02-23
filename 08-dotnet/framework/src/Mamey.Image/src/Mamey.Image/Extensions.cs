using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mamey.Image.BackgroundRemoval;

namespace Mamey.Image;

public static class Extensions
{
    public static IMameyBuilder AddMameyImage(this IMameyBuilder builder)
    {
        return builder;
    }

    /// <summary>
    /// Adds background removal services to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration section for background removal options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyImageBackgroundRemoval(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
            
        // Configure options
        services.Configure<BackgroundRemovalOptions>(configuration.GetSection("BackgroundRemovalApi"));
        
        // Register HTTP client
        services.AddHttpClient<IBackgroundRemovalClient, BackgroundRemovalClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BackgroundRemovalOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }

    /// <summary>
    /// Adds background removal services to the service collection with custom options.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure background removal options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMameyImageBackgroundRemoval(
        this IServiceCollection services,
        Action<BackgroundRemovalOptions> configureOptions)
    {
        // Configure options
        services.Configure(configureOptions);
        
        // Register HTTP client
        services.AddHttpClient<IBackgroundRemovalClient, BackgroundRemovalClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BackgroundRemovalOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
