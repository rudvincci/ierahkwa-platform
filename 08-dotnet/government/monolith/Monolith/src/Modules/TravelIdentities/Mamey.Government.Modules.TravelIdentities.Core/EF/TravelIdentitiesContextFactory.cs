using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF;

internal class TravelIdentitiesContextFactory : IDesignTimeDbContextFactory<TravelIdentitiesDbContext>
{
    public TravelIdentitiesDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<TravelIdentitiesDbContext>();
        
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.Government.Modules.TravelIdentities.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_TravelIdentities", "travel_identities");
        });

        return new TravelIdentitiesDbContext(optionsBuilder.Options);
    }
}
