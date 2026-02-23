using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Government.Modules.Citizens.Core.EF;

internal class CitizensContextFactory : IDesignTimeDbContextFactory<CitizensDbContext>
{
    public CitizensDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<CitizensDbContext>();
        
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.Government.Modules.Citizens.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Citizens", "citizens");
        });

        return new CitizensDbContext(optionsBuilder.Options);
    }
}
