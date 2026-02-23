namespace Mamey.Government.BlazorServer.Seeding;

/// <summary>
/// Extension methods for data seeding.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers the data seeder orchestrator.
    /// </summary>
    public static IServiceCollection AddDataSeeding(this IServiceCollection services)
    {
        services.AddSingleton<DataSeederOrchestrator>();
        return services;
    }

    /// <summary>
    /// Seeds the database with initial data if in development environment.
    /// </summary>
    public static async Task<IApplicationBuilder> UseDevelopmentSeedingAsync(
        this IApplicationBuilder app, 
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        // Only seed in development or if explicitly enabled
        var shouldSeed = environment.IsDevelopment() 
            || configuration.GetValue<bool>("Seeding:Enabled", false);

        if (!shouldSeed)
        {
            return app;
        }

        var logger = app.ApplicationServices.GetRequiredService<ILogger<DataSeederOrchestrator>>();
        logger.LogInformation("Development data seeding is enabled");

        try
        {
            var seeder = app.ApplicationServices.GetRequiredService<DataSeederOrchestrator>();
            await seeder.SeedAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed development data. Continuing without seed data.");
        }

        return app;
    }
}
