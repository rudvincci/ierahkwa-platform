using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.ServiceName.Infrastructure.EF;

internal class EntityNameContextFactory : IDesignTimeDbContextFactory<EntityNameDbContext>
{
    public EntityNameDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<EntityNameDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new EntityNameDbContext(optionsBuilder.Options);
    }
}