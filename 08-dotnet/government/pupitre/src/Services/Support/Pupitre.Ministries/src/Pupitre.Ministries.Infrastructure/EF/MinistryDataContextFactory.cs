using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pupitre.Ministries.Infrastructure.EF;

internal class MinistryDataContextFactory : IDesignTimeDbContextFactory<MinistryDataDbContext>
{
    public MinistryDataDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<MinistryDataDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new MinistryDataDbContext(optionsBuilder.Options);
    }
}