using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Government.Modules.Identity.Core.EF;

internal class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.Government.Modules.Identity.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "identity");
        });

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
