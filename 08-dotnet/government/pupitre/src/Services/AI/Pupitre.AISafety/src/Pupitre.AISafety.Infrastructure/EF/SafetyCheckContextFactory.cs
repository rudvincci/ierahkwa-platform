using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pupitre.AISafety.Infrastructure.EF;

internal class SafetyCheckContextFactory : IDesignTimeDbContextFactory<SafetyCheckDbContext>
{
    public SafetyCheckDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<SafetyCheckDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new SafetyCheckDbContext(optionsBuilder.Options);
    }
}